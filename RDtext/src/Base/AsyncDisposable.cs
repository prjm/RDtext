using System;
using System.Threading;
using System.Threading.Tasks;
using RDtext.Attributes;

namespace RDtext.Base {

    /// <summary>
    ///     base class for asynchronous disposable objects
    /// </summary>
    [Mutable]
    public class AsyncDisposable : IAsyncDisposable {

        private volatile int isDisposed;

        /// <summary>
        ///     dispose this object asynchronously
        /// </summary>
        /// <returns></returns>
        protected virtual ValueTask DoDisposeAsync()
            => default;

        /// <summary>
        ///     dispose this object
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync() {
            if (Interlocked.CompareExchange(ref isDisposed, 1, 0) != 0)
                return;

            await DoDisposeAsync().NoSync();
        }

        /// <summary>
        ///     guard: throws an exception if the object is already disposed
        /// </summary>
        protected void ThrowIfObjectDisposed() {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        ///     check if this object is already disposed
        /// </summary>
        public bool IsDisposed
            => isDisposed > 0;
    }
}
