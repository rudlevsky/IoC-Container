using System;
using System.Collections.Generic;

namespace Simple_IoC_Container.IoCContainer.Interfaces
{
    public interface ISimpleIoC
    {
        T Resolve<T>();
        void UseSingleton();
        Dictionary<Type, Type> GetAllRegisteredTypes();
        bool IsRegistered<Type>();
    }
}