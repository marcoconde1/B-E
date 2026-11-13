using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace TGFramework
{
    /// <summary>
    /// Simple global service locator allowing registration and retrieval of services by type.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        /// <summary>
        /// Registers a service instance for the given type <typeparamref name="T"/>.
        /// If a service of this type already exists, it will be replaced.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="service">The service instance.</param>
        public static void Register<T>(T service)
        {
            Type type = service.GetType();
            if (_services.ContainsKey(type))
            {
                _services.Remove(type);
            }
            _services.Add(type, service);
        }

        /// <summary>
        /// Gets the registered service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>The registered service instance.</returns>
        /// <exception cref="Exception">Thrown if the service was not found.</exception>
        public static T Get<T>()
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
            {
                return (T)_services[type];
            }
            
            throw new Exception("Service not found");
        }

        /// <summary>
        /// Tries to get the registered service of type <typeparamref name="T"/>.
        /// Returns <c>null</c> if the service isn't registered.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>The service instance if found; otherwise <c>null</c>.</returns>
        public static T TryGet<T>() where T : class
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
            {
                return (T)_services[type];
            }
            
            return null;
        }
    }
}