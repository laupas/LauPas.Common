using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scrutor;

namespace LauPas.Common
{
    public class Starter : IStarter
    {
        private IServiceProvider serviceProvider;
        private readonly ServiceCollection serviceCollection;
        private readonly List<Assembly> assemblies = new List<Assembly>();

        private static Starter CurrentStarter { get; set; }

        public static IStarter Create()
        {
            CurrentStarter =  new Starter();
            return CurrentStarter;
        }
        
        public  static IStarter Get => CurrentStarter;

        private Starter()
        {
            this.serviceCollection = new ServiceCollection();
        }

        public IStarter AddAssembly<TTypeInAssembly>()
        {
            this.assemblies.Add(typeof(TTypeInAssembly).Assembly);
            return this;
        }

        public IStarter Build(string[] args = null, Action<IServiceCollection> extend = null)
        {
            this.SetArguments(args);
            this.SetLogging();
            var startupLogger = this.serviceCollection
                .BuildServiceProvider()
                .GetService<ILoggerFactory>()
                .CreateLogger<IStarter>();

            
            this.assemblies.Add(this.GetType().Assembly);
            var tempAssemblies = this.assemblies.Distinct();
            foreach (var tempAssembly in tempAssemblies)
            {
                startupLogger.LogTrace($"Add Assembly {tempAssembly.FullName} to container.");
            }
            
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

        IStarter IStarter.AddAssembly(Assembly assembly)
        {
            this.assemblies.Add(assembly);
            return this;
        }
    }
}