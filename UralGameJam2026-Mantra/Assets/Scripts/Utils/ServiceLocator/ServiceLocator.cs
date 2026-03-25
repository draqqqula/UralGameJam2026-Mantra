using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance;

    [SerializeField] private WindowsService _windowsService;
    [SerializeField] private MatchResultHandler _matchResultHandler;
    [SerializeField] private Settings _settings;
    
    private Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(this);
        RegisterDefaultServices();
    }

    private void RegisterDefaultServices()
    {
        RegisterService(_windowsService);
        RegisterService(_matchResultHandler);
        RegisterService(_settings);
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