using System;

using RDtext.DataPooling;

namespace RDtext_tests {

    internal class TestPoolItem : PoolItem {
        public TestPoolItem(Pool owner) : base(owner) {
        }

        public int Value { get; set; }

        protected internal override void Clear()
            => Value = 0;
    }

    internal class TestPool : Pool<TestPoolItem> {
        protected override TestPoolItem CreateItem()
            => new TestPoolItem(this);
    }

    public class PoolTest : CommonTest {

        [TestMethod]
        public void TestSimplePool() {
            var simplePool = new TestPool();
            AssertError<ArgumentNullException>(() => simplePool.Return(null!));
            AssertEqual(0, simplePool.Count);

            var i1 = simplePool.Rent();
            var i2 = simplePool.Rent();
            i1.Value = 3;
            i2.Value = 4;
            AssertEqual(false, i1.IsInPool);
            AssertEqual(false, i2.IsInPool);
            AssertEqual(i1.ObjectPool, simplePool);

            i1.Dispose();
            i2.Dispose();
            AssertEqual(2, simplePool.Count);

            i1.Dispose();
            i2.Dispose();
            AssertEqual(2, simplePool.Count);

        }

        [TestMethod]
        public void TestArrayPool() {
            AssertError<ArgumentOutOfRangeException>(() => new FixedSizeArrayPool<byte>(-3));

            var arrayPool = new FixedSizeArrayPool<byte>(500);
            AssertEqual(0, arrayPool.Count);

            var item = arrayPool.Rent();
            AssertEqual(false, item.IsInPool);
            AssertEqual(0, arrayPool.Count);
            AssertEqual(500, item.Data.Length);

            item.Dispose();
            AssertEqual(true, item.IsInPool);
            AssertEqual(1, arrayPool.Count);

            item.Dispose();
            AssertEqual(true, item.IsInPool);
            AssertEqual(1, arrayPool.Count);
        }

    }
}
