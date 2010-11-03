using StructureMap.AutoNotify;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.UnitTests
{
    [TestClass]
    public class DependencyMapTests
    {
        [TestMethod]
        public void CanExtractSourceMemberFromExpression()
        {
            var map = new FooMap();

            Assert.IsTrue(map.Map[0].SourcePropName == "Value");
        }

        [TestMethod]
        public void CanExtractTargetMemberFromExpression()
        {
            var map = new FooMap();

            Assert.IsTrue(map.Map[0].TargetPropName == "StringValue");
        }

        [TestMethod]
        public void CanExtractSourceMemberFromNestedPropExpression()
        {
            var map = new FooMap();

            Assert.IsTrue(map.Map[1].SourcePropName == "Bar.Value");
        }

        class Foo
        {
            public virtual int Value{ get; set; }
            public virtual string StringValue { get { return Value.ToString(); } }

            public virtual Bar Bar { get; set; }
            public virtual int CombinedValue { get { return Value + Bar.Value; } }
        }

        class Bar
        {
            public virtual int Value { get; set; }
        }

        class FooMap : DependencyMap<Foo>
        {
            public FooMap()
            {
                Property(x => x.StringValue).DependsOn(x => x.Value);
                Property(x => x.CombinedValue).DependsOn(x => x.Bar.Value);
            }
        }
    }
}
