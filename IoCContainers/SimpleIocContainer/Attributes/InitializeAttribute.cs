using System;

namespace SimpleContainer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class InitializeAttribute : Attribute
    {
    }
}
