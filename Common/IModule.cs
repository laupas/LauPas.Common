using Microsoft.Extensions.DependencyInjection;

namespace LauPas.Common
{
    public interface IModule
    {
        void Extend(IServiceCollection serviceCollection);
    }
}