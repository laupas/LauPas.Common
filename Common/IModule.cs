using Microsoft.Extensions.DependencyInjection;

namespace LauPas.Common
{
    /// <summary>
    /// The IModule
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Define and overwrite special registrations for the container.
        /// </summary>
        /// <param name="serviceCollection">The serviceCollection</param>
        void Extend(IServiceCollection serviceCollection);
    }
}