using System;
using System.ComponentModel;
using Castle.DynamicProxy;
using StructureMap.AutoNotify;
using Tests.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.UnitTests
{
    [TestClass]
    public class CanMakeNotifiableForInterface
    {
        [TestMethod]
        public void ShouldReturnAnINPCForInterfacedObject()
        {
            var greeter = Notifiable.MakeForInterfaceGeneric<IGreeter>(new LolCat(), FireOptions.Always, new ProxyGenerator(), new DependencyMap());

            Assert.IsTrue(greeter is INotifyPropertyChanged);
        }

        [TestMethod]
        public void ShouldFireChangedWhenPropertyChangedOnMadeObject()
        {
            var greeter = Notifiable.MakeForInterfaceGeneric<IGreeter>(new LolCat(), FireOptions.Always, new ProxyGenerator(), new DependencyMap());

            var tracker = new EventTracker<PropertyChangedEventHandler>();

            (greeter as INotifyPropertyChanged).PropertyChanged += tracker;

            greeter.Greeting = "buzz off";

            Assert.IsTrue(tracker.WasCalled);
        }

        [TestMethod]
        public void ShouldThrowWhenGiveNonInterface()
        {
            var exceptionThrown = false;
            try
            {
                Notifiable.MakeForInterfaceGeneric<LolCat>(new LolCat(), FireOptions.Always, new ProxyGenerator(), new DependencyMap());
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        public class LolCat : IGreeter
        {
            public LolCat()
            {
                Greeting = "OHai";
            }

            public string Hello()
            {
                return Greeting;
            }

            public string Greeting { get; set; }
        }

        [AutoNotify]
        public interface IGreeter
        {
            string Hello();

            string Greeting { get; set; }
        }
    }
}