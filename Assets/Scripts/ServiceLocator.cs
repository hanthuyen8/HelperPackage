using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Scripting;

namespace C18 {
    internal class ServiceLocatorImpl {
        private readonly Dictionary<string, IService> _services = new();
        private readonly ServiceNameCache _nameCache = new();

        public void Provide(IService service) {
            var type = service.GetType();
            var name = _nameCache.GetServiceName(type);
            if (_services.TryGetValue(name, out var currentService)) {
                currentService.Destroy();
            }
            _services.Remove(name);
            _services.Add(name, service);
        }

        public T Resolve<T>() where T : IService {
            var name = _nameCache.GetServiceName<T>();
            if (_services.TryGetValue(name, out var item)) {
                if (item is T service) {
                    return service;
                }
            }
            throw new Exception($"Cannot find the requested service: {name}");
        }
    }

    public static class ServiceLocator {
        private static readonly ServiceLocatorImpl Impl = new();

        /// <summary>
        /// Registers a service.
        /// </summary>
        /// <param name="service"></param>
        public static void Provide(IService service) {
            Impl.Provide(service);
        }

        /// <summary>
        /// Resolves the specified service.
        /// </summary>
        public static T Resolve<T>() where T : IService {
            return Impl.Resolve<T>();
        }
    }
    
    public class ServiceNameCache {
        private readonly Dictionary<Type, string> _serviceNames = new();

        public string GetServiceName<T>() where T : IService {
            return GetServiceName(typeof(T));
        }

        public string GetServiceName(Type type) {
            if (_serviceNames.TryGetValue(type, out var result)) {
                return result;
            }
            var interfaces = type.GetInterfaces().ToList();
            if (type.IsInterface) {
                interfaces.Add(type);
            }
            foreach (var item in interfaces) {
                var attribute = Attribute.GetCustomAttribute(item, typeof(ServiceAttribute));
                if (attribute is ServiceAttribute serviceAttribute) {
                    var name = serviceAttribute.Name;
                    _serviceNames.Add(type, name);
                    return name;
                }
            }
            throw new Exception($"The requested service is not registered: {type.Name}");
        }
    }
    
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceAttribute : Attribute {
        /// <summary>
        /// Gets the registered name of this service.
        /// </summary>
        public string Name { get; }

        public ServiceAttribute(string name) {
            Name = name;
        }
    }
    
    /// <summary>
    /// Base interface for all service classes.
    /// </summary>
    [Preserve]
    public interface IService {
        /// <summary>
        /// Asynchronously initializes this service.
        /// </summary>
        Task<bool> Initialize();

        /// <summary>
        /// Destroys this service.
        /// </summary>
        void Destroy();
    }
}