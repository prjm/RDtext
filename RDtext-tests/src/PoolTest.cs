using System;

using RDtext.DataPooling;

namespace RDtext.Tests {

    internal class TestPoolItem : PoolItem {
        public TestPoolItem(Pool owner) : base(owner) {
        }

        public int Value { get; set; }

        protected internal override void Clear()
            => Value = 0;
    }

    internal class CountObject : UsageCountedObject {
    }

    internal class TestPool : Pool<TestPoolItem> {
        protected override TestPoolItem CreateItem()
            => new(this);
    }

    public class PoolTest : CommonTest {

        [TestMethod]
        public void TestSimplePool() {
            var simplePool = new TestPool();
            AssertError<ArgumentNullException>(() => simplePool.ReturnPoolItem(null!));
            AssertEqual(0, simplePool.Count);

            var i1 = simplePool.Rent();
            var i2 = simplePool.Rent();
            i1.Value = 3;
            i2.Value = 4;
            AssertEqual(true, i1.IsPinned);
            AssertEqual(true, i2.IsPinned);
            AssertEqual(i1.ObjectPool, simplePool);
            AssertEqual(0, simplePool.Count);

            i1.UnPin();
            i2.UnPin();
            AssertEqual(2, simplePool.Count);
        }

        [TestMethod]
        public void TestArrayPool() {
            AssertError<ArgumentOutOfRangeException>(() => _ = new FixedSizeArrayPool<byte>(-3));

            var arrayPool = new FixedSizeArrayPool<byte>(500);
            AssertEqual(0, arrayPool.Count);

            var item = arrayPool.Rent();
            AssertEqual(true, item.IsPinned);
            AssertEqual(0, arrayPool.Count);
            AssertEqual(500, item.GetData().Length);

            item.UnPin();
            AssertEqual(false, item.IsPinned);
            AssertEqual(1, arrayPool.Count);
        }


        [TestMethod]
        public void TestUseAndReturn() {
            using var demoObject = new CountObject();
            AssertEqual(false, demoObject.IsPinned);
            demoObject.Pin();
            AssertEqual(true, demoObject.IsPinned);
            using (var x = UseAndReturn.That(demoObject)) {
                AssertEqual(true, demoObject.IsPinned);
            }
            AssertEqual(false, demoObject.IsPinned);

        }

    }
}
