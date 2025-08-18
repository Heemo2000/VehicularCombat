using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class ServiceManager
    {
        private Dictionary<Type, object> registeredServicesMap = new Dictionary<Type, object>();
        public IEnumerable<object> RegisteredServices => registeredServicesMap.Values;

        
        public bool TryGet<T>(out T service) where T : class
        {
            Type type = typeof(T);   
            if(registeredServicesMap.TryGetValue(type, out object serviceObj))
            {
                service = serviceObj as T;
                return true;
            }

            service = null;
            return false;
        }

        public T Get<T>() where T: class
        {
            Type type = typeof(T);
            if (registeredServicesMap.TryGetValue(type, out object obj))
            {
                return obj as T;
            }

            throw new ArgumentException($"ServiceManager.Get: Service of type {type.FullName} not registered");
        }
        public ServiceManager Register<T>(T service)
        {
            Type type = typeof(T);

            if(!registeredServicesMap.TryAdd(type, service))
            {
                Debug.LogError("Service of type " + type.FullName + " already registered");
            }

            Debug.Log($"Object of type {type.FullName} registered.");
            return this;
        }

        public ServiceManager Register(Type type, object service)
        {
            if(!type.IsInstanceOfType(service))
            {
                Debug.LogError("Type does not match the type of service interface mentioned: " + nameof(service));
            }

            if (!registeredServicesMap.TryAdd(type, service))
            {
                Debug.LogError("Service of type " + type.FullName + " already registered");
            }

            Debug.Log($"Object of type {type.FullName} registered.");
            return this;
        }
    }
}
