using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scrutor;

namespace LauPas.Common
{
    /// <summary>
    /// The Starter
    /// </summary>
    public class Starter : IStarterBuilder, IServiceLocator
    {
        private IServiceProvider serviceProvider;
        private readonly ServiceCollection serviceCollection;
        private readonly List<Assembly> assemblies = new List<Assembly>();
        private readonly List<Tuple<Type,object>> modules = new List<Tuple<Type, object>>();

        private static Starter CurrentStarter { get; set; }

        /// <summary>
        /// Create the starter instance.
        /// </summary>
        /// <returns></returns>
        public static IStarterBuilder Create()
        {
            CurrentStarter =  new Starter();
            return CurrentStarter;
        }
        
        /// <summary>
        /// Get the current instance of the servicelocator
        /// </summary>
        public  static IServiceLocator Get => CurrentStarter;

        private Starter()
        {
            this.serviceCollection = new ServiceCollection();
        }
        
        /// <inheritdoc />
        public IStarterBuilder AddAssembly(Assembly assembly)
        {
            this.assemblies.Add(assembly);
            return this;
        }

        /// <inheritdoc />
        public IStarterBuilder AddAssembly<TTypeInAssembly>()
        {
            this.assemblies.Add(typeof(TTypeInAssembly).Assembly);
            return this;
        }

        /// <inheritdoc />
        public IStarterBuilder AddModule<TModule>() where TModule : IModule
        {
            this.assemblies.Add(typeof(TModule).Assembly);
            this.modules.Add(new Tuple<Type, object>(typeof(TModule), null));
            return this;
        }

        /// <inheritdoc />
        public IStarterBuilder AddModule<TModule, TArgument>(TArgument argument) where TModule : IModule<TArgument>
        {
            this.assemblies.Add(typeof(TModule).Assembly);
            this.modules.Add(new Tuple<Type, object>(typeof(TModule), argument));
            return this;
        }

        /// <inheritdoc />
        public IServiceLocator Build(string[] args = null, Action<IServiceCollection> extend = null)
        {
            this.SetArguments(args);
            this.SetLogging();
            
            this.assemblies.Add(this.GetType().Assembly);
            var tempAssemblies = this.assemblies.Distinct();
            
            this.serviceCollection.Scan(scan =>
                scan.FromAssemblies(tempAssemblies)
                    .AddClasses(c => c.WithoutAttribute<Singleton>())
                    .UsingRegistrationStrategy(RegistrationStrategy.Append)
                    .AsSelfWithInterfaces()
                    .AsSelf()
                    .WithTransientLifetime());

            this.serviceCollection.Scan(scan =>
                scan.FromAssemblies(tempAssemblies)
                    .AddClasses(c => c.WithAttribute<Singleton>())
                    .UsingRegistrationStrategy(RegistrationStrategy.Append)
                    .AsSelfWithInterfaces()
                    .AsSelf()
                    .WithSingletonLifetime());
            
            extend?.Invoke(this.serviceCollection);
            
            var tempServiceProvider = this.serviceCollection
                .BuildServiceProvider();
            var startupLogger = tempServiceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger<IStarterBuilder>();

            // Only for logging
            foreach (var tempAssembly in tempAssemblies)
            {
                startupLogger.LogTrace($"Add Assembly {tempAssembly.FullName} to container.");
            }

            foreach (var module in this.modules)
            {
                var moduleInstance = tempServiceProvider.GetService(module.Item1);
                if (module.Item2 == null)
                {
                    moduleInstance.GetType().GetMethod("Extend").Invoke(moduleInstance, new[] {this.serviceCollection});
                }
                else
                {
                    moduleInstance.GetType().GetMethod("Extend").Invoke(moduleInstance,
                        new object[] {this.serviceCollection, module.Item2});
                }
            }
            
            this.serviceProvider = this.serviceCollection.BuildServiceProvider();
            return this;
        }

        private void SetLogging()
        {
            this.serviceCollection.AddLogging(builder => builder
                .AddFilter(level =>
                {
                    var verbose = Environment.GetEnvironmentVariable("VERBOSE");
                    if (!string.IsNullOrEmpty(verbose))
                    {
                        return level >= LogLevel.Trace;
                    }

                    return level >= LogLevel.Information;
                })
            );
        }

        private void SetArguments(string[] args)
        {
            args?.ToList().ForEach(arg =>
            {
                if (arg.StartsWith("--"))
                {
                    if (arg.Contains("="))
                    {
                        var temp = arg.Split('=');
                        var key = temp[0].Substring(2);
                        var value = string.Join("=", temp.Skip(1));
                        Environment.SetEnvironmentVariable(key.ToUpperInvariant(), value);
                    }
                    else
                    {
                        var temp = arg.Split('=');
                        var key = temp[0].Substring(2);
                        Environment.SetEnvironmentVariable(key.ToUpperInvariant(), "true");
                    }
                }
            });
        }

        /// <inheritdoc />
        public T Resolve<T>()
        {
            var result =  this.serviceProvider.GetService<T>();
            if (result == null)
            {
                throw new Exception($"Type {typeof(T)} can not bre resolved.");
            }
            return result;
        }
        
    }
}