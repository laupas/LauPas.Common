using System;

namespace LauPas.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Singleton: Attribute
    {
    }
}