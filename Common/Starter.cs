using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scrutor;

namespace LauPas.Common
{
    public class Starter : IStarterBuilder, IServiceLocator
    {
        private IServiceProvider serviceProvider;
        private readonly ServiceCollection serviceCollection;
        private readonly List<Assembly> assemblies = new List<Assembly>();
        private readonly List<Type> modules = new List<Type>();

        private static Starter CurrentStarter { get; set; }

        public static IStarterBuilder Create()
        {
            CurrentStarter =  new Starter();
            return CurrentStarter;
        }
        
        public  static IServiceLocator Get => CurrentStarter;

        private Starter()
        {
            this.serviceCollection = new ServiceCollection();
        }

        public IStarterBuilder AddAssembly(Assembly assembly)
        {
            this.assemblies.Add(assembly);
            return this;
        }

        public IStarterBuilder AddAssembly<TTypeInAssembly>()
        {
            this.assemblies.Add(typeof(TTypeInAssembly).Assembly);
            return this;
        }

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
                var moduleInstance = (IModule)tempServiceProvider.GetService(module);
                moduleInstance.Extend(this.serviceCollection);
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

        public T Resolve<T>()
        {
            var result =  this.serviceProvider.GetService<T>();
            if (result == null)
            {
                throw new Exception($"Type {typeof(T)} can not bre resolved.");
            }
            return result;
        }

        public IStarterBuilder AddModule<TModule>() where TModule : IModule
        {
            this.assemblies.Add(typeof(TModule).Assembly);
            this.modules.Add(typeof(TModule));
            return this;
        }

    }
}