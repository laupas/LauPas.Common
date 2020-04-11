using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LauPas.Common
{
    public interface IServiceLocator
    {
        T Resolve<T>();
    }
    
    public interface IStarterBuilder
    {
        IStarterBuilder AddModule<TModule>() where TModule : IModule;
        IStarterBuilder AddAssembly(Assembly assembly);
        IStarterBuilder AddAssembly<TTypeInAssembly>();
        IServiceLocator Build(string[] args = null, Action<IServiceCollection> extend = null);
    }

}