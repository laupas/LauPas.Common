using System;

namespace LauPas.Common
{
    /// <summary>
    /// Adding this Attribute to a class will resolve it as a singleton instance from the container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Singleton: Attribute
    {
    }
}