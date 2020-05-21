using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LauPas.Common
{
    /// <summary>
    /// The Starter Builder interface
    /// </summary>
    public interface IStarterBuilder
    {
        /// <summary>
        /// Add a module to the container.
        /// </summary>
        /// <typeparam name="TModule">The module</typeparam>
        /// <returns>The current IStarterBuilder</returns>
        IStarterBuilder AddModule<TModule>() where TModule : IModule;

        /// <summary>
        /// Add a module to the container.
        /// </summary>
        /// <typeparam name="TModule">The module</typeparam>
        /// <typeparam name="TArgument">The arguments</typeparam>
        /// <returns>The current IStarterBuilder</returns>
        IStarterBuilder AddModule<TModule, TArgument>(TArgument argument) where TModule : IModule<TArgument>;
        
        /// <summary>
        /// Add an Assembly to the container.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>The current IStarterBuilder</returns>
        IStarterBuilder AddAssembly(Assembly assembly);

        /// <summary>
        /// Add an Assembly to the container.
        /// </summary>
        /// <typeparam name="TTypeInAssembly">One type into the assembly</typeparam>
        /// <returns>The current IStarterBuilder</returns>
        IStarterBuilder AddAssembly<TTypeInAssembly>();
        
        /// <summary>
        /// Builds the container.
        /// All registration and definitions must be done before this call.
        /// </summary>
        /// <param name="args">Command line Arguments</param>
        /// <param name="extend">The last possibility to define and change registrations</param>
        /// <returns></returns>
        IServiceLocator Build(string[] args = null, Action<IServiceCollection> extend = null);
    }

}