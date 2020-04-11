using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LauPas.Common
{
    public interface IStarter
    {
        IStarter AddAssembly(Assembly assembly);
        IStarter AddAssembly<TTypeInAssembly>();
        IStarter Build(string[] args = null, Action<IServiceCollection> extend = null);
        T Resolve<T>();
    }
}