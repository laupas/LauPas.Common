using System;
using Microsoft.Extensions.DependencyInjection;

namespace LauPas.Common
{
    public interface IStarter
    {
        IStarter AddAssembly(object referenceObject);
        IStarter AddAssembly<TTypeInAssembly>();
        IStarter Build(string[] args = null, Action<IServiceCollection> extend = null);
        T Resolve<T>();
    }
}