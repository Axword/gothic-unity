using System;
using System.Collections.Generic;

namespace ZelaznaDroga.Core.Utilities
{
    /// <summary>
    /// Simple service locator for dependency injection.
    /// Provides a static way to access game services.
    /// </summary>
    public static class ComponentLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, List<object>> _typedServices = new Dictionary<Type, List<object>>();

        /// <summary>
        /// Registers a service.
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            _services[typeof(T)] = service;
            
            if (!_typedServices.ContainsKey(typeof(T)))
            {
                _typedServices[typeof(T)] = new List<object>();
            }
            
            if (!_typedServices[typeof(T)].Contains(service))
            {
                _typedServices[typeof(T)].Add(service);
            }
        }

        /// <summary>
        /// Unregisters a service.
        /// </summary>
        public static void Unregister<T>(T service) where T : class
        {
            _services.Remove(typeof(T));
            _typedServices[typeof(T)]?.Remove(service);
        }

        /// <summary>
        /// Gets a registered service.
        /// </summary>
        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out object service))
            {
                return (T)service;
            }
            return null;
        }

        /// <summary>
        /// Gets a registered service or throws if not found.
        /// </summary>
        public static T Require<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out object service))
            {
                return (T)service;
            }
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered!");
        }

        /// <summary>
        /// Checks if a service is registered.
        /// </summary>
        public static bool IsRegistered<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Gets all services of a specific type (for interface implementations).
        /// </summary>
        public static IReadOnlyList<T> GetAll<T>() where T : class
        {
            if (_typedServices.TryGetValue(typeof(T), out List<object> services))
            {
                List<T> result = new List<T>();
                foreach (object service in services)
                {
                    if (service is T typedService)
                    {
                        result.Add(typedService);
                    }
                }
                return result;
            }
            return new List<T>();
        }

        /// <summary>
        /// Clears all registered services.
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
            _typedServices.Clear();
        }
    }
}
