using System;
using System.Threading;
using System.Threading.Tasks;
using RDtext.Attributes;

namespace RDtext.Base {

    /// <summary>
    ///     base class for usage counted objects
    /// </summary>
    [Mutable]
    public abstract class UsageCountedObject : AsyncDisposable {

        private volatile int usageCount;

        /// <summary>
        ///     pin this object
        /// </summary>
        public void Pin() {
            ThrowIfObjectDisposed();
            var count = Interlocked.Increment(ref usageCount);
            if (count < 0)
                throw new OverflowException();
        }

        /// <summary>
        ///     unpin this object
        /// </summary>
        public async ValueTask UnPin(CancellationToken cancellationToken = default) {
            ThrowIfObjectDisposed();

            var count = Interlocked.Decrement(ref usageCount);

            if (count < 0)
                throw new OverflowException();

            await DoUnPin(count > 0, cancellationToken).NoSync();
        }

        /// <summary>
        ///     action on unpin
        /// </summary>
        protected virtual ValueTask DoUnPin(bool isPinned, CancellationToken cancellationToken)
            => default;

        /// <summary>
        ///     check if this object is in use
        /// </summary>
        public bool IsPinned {

            get {
                ThrowIfObjectDisposed();
                return usageCount > 0;
            }
        }

    }
}
