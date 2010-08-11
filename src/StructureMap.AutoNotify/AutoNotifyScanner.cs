﻿using System;
using System.Linq;
using Castle.DynamicProxy;
using log4net;
using StructureMap.Graph;

namespace StructureMap.AutoNotify
{
    public class AutoNotifyScanner : ITypeScanner
    {
        static readonly ILog logger = LogManager.GetLogger(typeof(AutoNotifyScanner));

        public void Process(Type type, PluginGraph graph)
        {
            if(type.IsEnum || !type.HasAttribute<AutoNotifyAttribute>())
                return;

            logger.InfoFormat("Registering autonotify for {0}", type.Name);

            if(type.IsInterface)
                ConfigureInterface(type, graph);
            else if(!type.IsAbstract)
                ConfigureClass(type, graph);
        }

        private void ConfigureInterface(Type type, PluginGraph graph)
        {
            graph.Configure(registry =>
            {
                registry
                    .For(type)
                    .EnrichWith((context, obj) => Notifiable.MakeForInterface(type, obj, new ProxyGenerator()));
            });
        }

        private void ConfigureClass(Type type, PluginGraph graph)
        {
            graph.Configure(registry =>
            {
                var inst = new LooseConstructorInstance(context =>
                {
                    var ctorArgs = type
                        .GetGreediestCtor()
                        .GetParameters()
                        .Select(p => context.GetInstance(p.ParameterType));

                    return Notifiable.MakeForClass(type, ctorArgs.ToArray(), new ProxyGenerator());
                });

                registry.For(type).Use(inst);
            });
        }
    }
}