﻿using System;
using System.ComponentModel;
using System.Linq;
using Castle.Core;
using Castle.DynamicProxy;
using log4net;
using AutoNotify.Extensions;

namespace AutoNotify
{
    public class Notifiable
    {
        static readonly ILog logger = LogManager.GetLogger("Notifiable");

        public static object MakeForInterface(Type type, object obj, FireOptions fireOption, ProxyGenerator generator, DependencyMap dependencyMap)
        {
            var maker = typeof(Notifiable).GetMethod("MakeForInterfaceGeneric");
            var typed = maker.MakeGenericMethod(type);
            return typed.Invoke(null, new[] { obj, fireOption, generator, dependencyMap });
        }

        public static T MakeForInterfaceGeneric<T>(T obj, FireOptions fireOption, ProxyGenerator generator, DependencyMap dependencyMap) where T : class
        {
            if(!typeof(T).IsInterface)
                throw new InvalidOperationException(string.Format("{0} is not an interface", typeof(T).Name));

            return (T)generator.CreateInterfaceProxyWithTarget(
                typeof(T),
                new[] { typeof(INotifyPropertyChanged) },
                obj,
                new PropertyChangedInterceptor(fireOption, dependencyMap));
        }

        public static object MakeForClass(Type type, FireOptions fireOption, object[] ctorArgs, ProxyGenerator generator, DependencyMap dependencyMap)
        {
            var maker = typeof(Notifiable).GetMethod("MakeForClassGeneric");
            var typed = maker.MakeGenericMethod(type);
            return typed.Invoke(null, new object[] { fireOption, generator, dependencyMap, ctorArgs });
        }

        public static T Make<T>(params object[] ctorArgs) where T : class
        {
            return MakeForClassGeneric<T>(FireOptions.OnlyOnChange, null, null, ctorArgs);
        }

        public static T MakeForClassGeneric<T>(FireOptions fireOption = FireOptions.OnlyOnChange, ProxyGenerator generator = null, DependencyMap dependencyMap = null, params object[] ctorArgs) where T : class
        {
            if (generator == null)
                generator = new ProxyGenerator();
            if (dependencyMap == null)
                dependencyMap = new DependencyMap();

            var nonVirtualProps = typeof(T)
                .GetProperties()
                .Select(prop => new { prop.Name, Setter = prop.GetSetMethod() })
                .Where(prop => prop.Setter != null && !prop.Setter.IsVirtual)
                .Select(prop => prop.Name);

            if(nonVirtualProps.IsNotEmpty())
            {
                logger.DebugFormat("Autonotify will not work for the following members on {0}.  Make the properties virtual to enable autonotify. {1}",
                    typeof(T).Name,
                    nonVirtualProps.Join(", "));
            }

            return (T)generator.CreateClassProxy(
                typeof(T),
                new[] { typeof(INotifyPropertyChanged) },
                ProxyGenerationOptions.Default,
                ctorArgs,
                new PropertyChangedInterceptor(fireOption, dependencyMap));
        }
    }
}