using System;
using System.Collections.Generic;

namespace SimpleContainer.Interfaces
{
    /// <summary>
    /// Contains methods for simple IoC container.
    /// </summary>
    public interface ISimpleIoC
    {
        T Resolve<T>();
        void UseSingleton();
        void NotUseSingleton();
        Dictionary<Type, Type> GetAllRegisteredTypes();
        bool IsRegistered<Type>();
    }
}
