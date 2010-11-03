using System.ComponentModel;
using Castle.DynamicProxy;
using AutoNotify;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Examples.NotifiableUsage
{
    [TestClass]
    public class NotifiableForInterface
    {
        [TestMethod]
        public void CanMakeAnObjectNotifiableWithInterface()
        {
            var foo = new Foo();
            var notifiableFoo = Notifiable.MakeForInterface(typeof(IFoo), foo, FireOptions.Always, new ProxyGenerator(), DependencyMap.Empty);

            Assert.IsTrue(notifiableFoo is INotifyPropertyChanged);
        }

        public interface IFoo
        {
            string Value { get; set; }
        }

        internal class Foo : IFoo
        {
            public string Value { get; set; }
        }
    }
}
