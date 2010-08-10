﻿using System.ComponentModel;
using NUnit.Framework;
using StructureMap.AutoNotify;
using Container = StructureMap.Container;

namespace Tests
{
    [TestFixture]
    public class AutoNotifyScannerTests
    {
        [TestCase]
        public void ShouldPickUpAutoNotifyAttributedClass()
        {
            var container = new Container(config => config.Scan(scan =>
            {
                scan.With<AutoNotifyScanner>();
                scan.TheCallingAssembly();
            }));

            Assert.That(container.GetInstance<TestClassWithAttr>(), Is.InstanceOf<INotifyPropertyChanged>());
        }

        [TestCase]
        public void ShouldNotPickUpNonAttributedClass()
        {
            var container = new Container(config => config.Scan(scan =>
            {
                scan.With<AutoNotifyScanner>();
                scan.TheCallingAssembly();
            }));

            Assert.That(container.GetInstance<TestClassWithoutAttr>(), Is.Not.InstanceOf<INotifyPropertyChanged>());
        }
    }

    [AutoNotify]
    public class TestClassWithAttr
    { }

    public class TestClassWithoutAttr
    { }
}