using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager
{
    private readonly Dictionary<Type, object> _services = new();
    public IEnumerable<object> RegisterServices => _services.Values;

    public ServiceManager Register<T>(T service)
    {
        var type = typeof(T);
        
        if (!_services.TryAdd(type, service))
            Debug.LogError($"{type.Name} is already registered");

        return this;
    }

    public ServiceManager Register(Type type, object service)
    {
        if (!type.IsInstanceOfType(service))
        {
            Debug.LogError($"{type.Name} is not assignable from {service.GetType().Name}");
            return this;
        }
        
        if (!_services.TryAdd(type, service))
            Debug.LogError($"{type.Name} is already registered");
        
        return this;
    }

    public T Get<T>() where T : class
    {
        var type = typeof(T);
        if (!_services.TryGetValue(type, out object service))
        {
            Debug.LogError($"{type.Name} is not registered");
            return null;
        }
        
        return service as T;
    }
    
    public bool TryGet<T>(out T service) where T : class
    {
        var type = typeof(T);
        if (!_services.TryGetValue(type, out object obj))
        {
            service = null;
            return false;
        }
        
        service = obj as T;
        return true;
    }
}












