using Simple_IoC_Container.IoCContainer.Attributes;
using Simple_IoC_Container.IoCContainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simple_IoC_Container.IoCContainer
{
    /// <summary>
    /// Simple IoC Container based on constructor injection.
    /// Just created for learning purpose.
    /// </summary>
    public class SimpleIoC : ISimpleIoC
    {
        Dictionary<Type, Type> registeredTypes = new Dictionary<Type, Type>();
        Dictionary<Type, object> instances = new Dictionary<Type, object>();

        bool isSingletonMode;

        /// <summary>
        /// Create an instance of the IoC Container
        /// </summary>
        /// <returns></returns>
        public static SimpleIoC CreateInstance() => new SimpleIoC();

        public T Resolve<T>() 
        {
            if (!registeredTypes.Any())
            {
                throw new Exception("No entity has been registered yet.");
            }

            //Get constructor
            T obj = (T)ResolveParameter(typeof(T));

            if (isSingletonMode && !instances.ContainsKey(typeof(T)))
            {
                instances.Add(typeof(T), obj);
            }

            return obj;
        }

        private object ResolveParameter(Type type)
        {
            try
            {
                Type resolved = null;

                if (isSingletonMode && instances.ContainsKey(type))
                {
                    return instances.First(f => f.Key == type).Value;
                }
                else
                {
                    resolved = registeredTypes[type];
                }          

                // var cnstr = resolved.GetConstructors().First();
                var cnstr = resolved.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
                var cnstrParams = cnstr.GetParameters().Where(w => w.GetType().IsClass);

                // If constructor hasn't parameter, Create an instance of object
                if (!cnstrParams.Any())
                    return Activator.CreateInstance(resolved);

                var paramLst = new List<object>(cnstrParams.Count());

                // Iterate through parameters and resolve each parameter
                for (int i = 0; i < cnstrParams.Count(); i++)
                {
                    var paramType = cnstrParams.ElementAt(i).ParameterType;
                    var resolvedParam = ResolveParameter(paramType);
                    Initialize(resolvedParam);
                    paramLst.Add(resolvedParam);
                }

                return cnstr.Invoke(paramLst.ToArray());
            }
            catch (Exception)
            {
                var err = string.Format("'{0}' Cannot be resolved. Check your registered types", type.FullName);
                throw new Exception(err);
            }

        }

        /// <summary>
        /// Private method for object validation.
        /// </summary>
        /// <param name="value">Object needed to be validated.</param>
        /// <returns><see cref="ValidationResult"/> object that represent the result of validation.</returns>
        private void Initialize<T>(T value)
        {
            //Searching for all class properties attributes
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

        /// <summary>
        /// Method for property validation.
        /// </summary>
        private bool ValidateProperty(PropertyInfo property)
        => property.GetCustomAttributes(typeof(InitializeAttribute), false).Length > 0 ? true : false;

        public void Register<TFrom, TTo>() 
            where TFrom : class 
            where TTo: class
        {
            if (registeredTypes.ContainsKey(typeof(TFrom)))
            {
                registeredTypes[typeof(TFrom)] = typeof(TTo);
            }
            else
            {
                registeredTypes.Add(typeof(TFrom), typeof(TTo));
            }
        }

        /// <summary>
        /// Use singleton for instantiating?
        /// </summary>
        /// <returns></returns>
        public void UseSingleton()
        {
            isSingletonMode = true;
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
        => registeredTypes.Any(a => a.Key == typeof(Type));
    }
}
