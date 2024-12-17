﻿using System;
using System.Collections.Generic;
using Utilities;
using View;

namespace Utilities
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();
        
        public static void Register<T>(T service)
        {
            _services[typeof(T)] = service;
        }
        
        public static void RegisterAs<T>(T service, Type type)
        {
            _services[type] = service;
        }
        
        public static void Unregister<T>()
        {
            _services.Remove(typeof(T));
        }
        
        public static bool Contains<T>()
        {
            return _services.ContainsKey(typeof(T));
        }
        
        public static T Get<T>()
        {
            return (T) _services[typeof(T)];
        }
        
        public static void Clear()
        {
            _services.Clear();
        }
    }
}
public class GameInitializer
{
    public void Initialize()
    {
        // Создаем экземпляр VFXView
        VFXView vfxView = new VFXView();

        // Регистрируем VFXView в ServiceLocator
        ServiceLocator.Register<VFXView>(vfxView);
    }
}