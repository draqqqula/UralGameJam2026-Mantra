using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance;

    [SerializeField] private WindowsService _windowsService;
    
    private Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this);
            return;
        }
        
        DontDestroyOnLoad(this);
        RegisterDefaultServices();
    }

    private void RegisterDefaultServices()
    {
        RegisterService(_windowsService);
    }

    public void RegisterService(IService service)
    {
        if (_services.ContainsKey(service.GetType())) _services.Remove(service.GetType());
        _services.Add(service.GetType(), service);
    }

    public T GetService<T>()
    {
        _services.TryGetValue(typeof(T), out var service);
        return (T)service;
    }

}

public interface IService { }