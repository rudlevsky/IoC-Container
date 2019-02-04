using SimpleContainer.Attributes;
using SimpleContainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleContainer
{
    /// <summary>
    /// Simple IoC Container based on constructor injection.
    /// </summary>
    public class IoCContainer : ISimpleIoC
    {
        private Dictionary<Type, Type> registeredTypes = 
            new Dictionary<Type, Type>();

        private Dictionary<Type, object> instances = 
            new Dictionary<Type, object>();

        private bool isSingletonMode;
        private Type currentType;

        /// <summary>
        /// Create an instance of the IoC Container.
        /// </summary>
        /// <returns></returns>
        public static IoCContainer CreateInstance() => new IoCContainer();

        /// <summary>
        /// Starts generating a type.
        /// </summary>
        /// <typeparam name="T">Type which is a key.</typeparam>
        /// <returns>Creates object.</returns>
        public T Resolve<T>()
        {
            if (!registeredTypes.Any())
            {
                throw new ArgumentException("There is no registered types.");
            }

            currentType = typeof(T);

            T obj = (T)ResolveParameter(typeof(T));

            return obj;
        }

        public void Register<TKey, TTo>()
        where TKey : class
        where TTo : class
        {
            registeredTypes[typeof(TKey)] = typeof(TTo);
        }

        /// <summary>
        /// Use singleton for instantiating.
        /// </summary>
        public void UseSingleton()
        {
            isSingletonMode = true;
        }

        /// <summary>
        /// Not use singleton for instantiating.
        /// </summary>
        public void NotUseSingleton()
        {
            isSingletonMode = false;
        }

        /// <summary>
        /// Get all of the registered types for this container
        /// </summary>
        /// <returns></returns>
        public Dictionary<Type, Type> GetAllRegisteredTypes()
        => new Dictionary<Type, Type>(registeredTypes);

        /// <summary>
        /// Returns true if specified type were registered
        /// </summary>
        /// <typeparam name="Type">The type that you want to check if is registered in the container</typeparam>
        /// <returns>True/False</returns>
        public bool IsRegistered<Type>()
        => registeredTypes.ContainsKey(typeof(Type));

        //Hotfixies
        private object ResolveParameter(Type type)
        {
            if (!registeredTypes.ContainsKey(type))
            {
                throw new ArgumentException("Type is not registered.");
            }

            var result = TryGetSingleton(type);

            if (result != null)
            {
                return result;
            }

            Type initType = registeredTypes[type];

            if (initType == currentType)
            {
                throw new ArgumentException("Circulation type found.");
            }

            var cnstrParams = GetConstructorWithLongestParameterList(initType);

            var paramList = GetConstructorParams(cnstrParams);

            result = Activator.CreateInstance(initType, paramList.ToArray());
            Initialize(result);

            TryAddToInstances(type, result);

            return result;
        }

        //Some changes_1
        private void TryAddToInstances(Type type, object result)
        {
            if (isSingletonMode && !instances.ContainsKey(type))
            {
                instances.Add(type, result);
            }
        }

        //Some changes_2
        private object TryGetSingleton(Type type)
        {
            if (isSingletonMode && instances.ContainsKey(type))
            {
                return instances.First(f => f.Key == type).Value;
            }

            return null;
        }

        //Develop something
        private IEnumerable<object> GetConstructorParams(ParameterInfo[] parameters)
        {
            var paramList = new List<object>();

            foreach (var item in parameters)
            {
                var paramType = item.ParameterType;
                var resolvedParam = ResolveParameter(paramType);
                paramList.Add(resolvedParam);
            }

            return paramList;
        }

        //Develop something
        private ParameterInfo[] GetConstructorWithLongestParameterList(Type type) =>
            type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault().GetParameters();

        //Release changes
        private void Initialize<T>(T value)
        {
            foreach (var prop in value.GetType().GetProperties())
            {
                if (prop.CustomAttributes != null)
                {
                    if (ValidateProperty(prop))
                    {
                        prop.SetValue(value, ResolveParameter(prop.PropertyType));
                    }
                }
            }
        }

        //Some feature changes
        //Some feature changes_2
        private bool ValidateProperty(PropertyInfo property)
        => property.GetCustomAttributes(typeof(InitializeAttribute), false).Length > 0 ? true : false;
    }
}
