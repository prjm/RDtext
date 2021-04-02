using System;
using System.Threading.Tasks;
using RDtext.Base;

namespace RDtext.Tests {

    /// <summary>
    ///     test asynchronous disposables
    /// </summary>
    public class AsyncDisposableTest : CommonTest {

        internal class TestObject : AsyncDisposable {
            public bool WasDisposed { get; private set; }

            protected override ValueTask DoDisposeAsync() {
                WasDisposed = true;
                return default;
            }

            public ValueTask ForceDispose()
                => DisposeAsync();

            public void Ping()
                => ThrowIfObjectDisposed();

        }

        [TestMethod]
        public async Task TestDoubleDispose() {
            var t1 = new TestObject();
            AssertEqual(false, t1.WasDisposed);
            AssertEqual(false, t1.IsDisposed);

            await t1.ForceDispose().NoSync();
            AssertEqual(true, t1.WasDisposed);
            AssertEqual(true, t1.IsDisposed);
            await t1.ForceDispose().NoSync();
            await t1.DisposeAsync().NoSync();
        }

        [TestMethod]
        public async Task TestThrowOnDisposed() {
            await using var t1 = new TestObject();
            await t1.ForceDispose().NoSync();
            AssertError<ObjectDisposedException>(() => t1.Ping());
        }

    }
}
