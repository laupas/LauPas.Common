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
            return this.serviceProvider.GetService<T>();
        }

        IStarter IStarter.AddAssembly(object referenceObject)
        {
            this.assemblies.Add(referenceObject.GetType().Assembly);
            return this;
        }
    }
}