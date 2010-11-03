using System.ComponentModel;
using Castle.DynamicProxy;
using AutoNotify;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Examples.NotifiableUsage
{
    [TestClass]
    public class NotifiableForClass
    {
        [TestMethod]
        public void CanMakeAnObjectNotifiableWithClass()
        {
            var notifiableFoo = Notifiable.MakeForClass(typeof(Foo), FireOptions.Always, new object[0], new ProxyGenerator(), DependencyMap.Empty);

            Assert.IsTrue(notifiableFoo is INotifyPropertyChanged);
        }

        public class Foo
        {
            public virtual string Value { get; set; }
        }
    }
}
