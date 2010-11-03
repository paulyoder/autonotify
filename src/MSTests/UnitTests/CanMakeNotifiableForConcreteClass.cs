using System.ComponentModel;
using Castle.DynamicProxy;
using StructureMap.AutoNotify;
using Tests.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.UnitTests
{
    [TestClass]
    public class CanMakeNotifiableForConcreteClass
    {
        [TestMethod]
        public void ShouldReturnAnINPCForConcreteObjectNoCtorArgs()
        {
            var cat = Notifiable.MakeForClassGeneric<LolCat>(FireOptions.Always, new ProxyGenerator(), new DependencyMap());

            Assert.IsTrue(cat is INotifyPropertyChanged);
        }

        [TestMethod]
        public void ShouldReturnAnINPCForConcreteObjectWithCtorArgs()
        {
            var obj = Notifiable.MakeForClassGeneric<ClassWithDependency>(FireOptions.Always, new ProxyGenerator(), new DependencyMap(), new Dependency { Value = 4 });

            Assert.IsTrue(obj is INotifyPropertyChanged);
            Assert.IsTrue(obj.Dependency.Value == 4);
        }

        [TestMethod]
        public void ShouldFireChangedWhenVirtualPropertySetOnMadeObject()
        {
            var cat = Notifiable.MakeForClassGeneric<LolCat>(FireOptions.Always, new ProxyGenerator(), new DependencyMap());

            var tracker = new EventTracker<PropertyChangedEventHandler>();

            (cat as INotifyPropertyChanged).PropertyChanged += tracker;

            cat.Greeting = "buzz off";

            Assert.IsTrue(tracker.WasCalled);
        }

        [TestMethod]
        public void ShouldNotFireChangedWhenVirtualPropertySetAndChangedOnMadeObjectWithChangeOnlyOption()
        {
            var cat = Notifiable.MakeForClassGeneric<LolCat>(FireOptions.OnlyOnChange, new ProxyGenerator(), new DependencyMap());

            var tracker = new EventTracker<PropertyChangedEventHandler>();


            cat.Greeting = "value";
            (cat as INotifyPropertyChanged).PropertyChanged += tracker;
            cat.Greeting = "value";

            Assert.IsTrue(tracker.WasNotCalled);
        }

        [TestMethod]
        public void ShouldNotFireChangedWhenNonVirtualPropertyChangedOnMadeObject()
        {
            var cat = Notifiable.MakeForClassGeneric<LolCat>(FireOptions.Always, new ProxyGenerator(), new DependencyMap());

            var tracker = new EventTracker<PropertyChangedEventHandler>();

            (cat as INotifyPropertyChanged).PropertyChanged += tracker;

            cat.Color = "purple";

            Assert.IsTrue(tracker.WasNotCalled);
        }

        public class LolCat
        {
            public LolCat()
            {
                Greeting = "OHai";
            }

            public string Hello()
            {
                return Greeting;
            }

            public virtual string Greeting { get; set; }
            public string Color { get; set; }
        }

        public class ClassWithDependency
        {
            readonly Dependency _dependency;

            public ClassWithDependency(Dependency dependency)
            {
                _dependency = dependency;
            }

            public Dependency Dependency {get { return _dependency; }}
        }

        public class Dependency
        {
            public int Value { get; set; }
        }
    }
}
