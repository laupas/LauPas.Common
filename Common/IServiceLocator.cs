namespace LauPas.Common
{
    /// <summary>
    /// The ServiceLocator
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// Resolve a instance from the container.
        /// </summary>
        /// <typeparam name="T">The type to resolve</typeparam>
        /// <returns>The resolved instance</returns>
        T Resolve<T>();
    }
}