using System;

namespace Simple_IoC_Container.IoCContainer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class InitializeAttribute : Attribute
    {
    }
}
