using System;
using System.Threading;
using System.Threading.Tasks;

namespace RDtext.DataPooling {

    /// <summary>
    ///     base class for usage counted objects
    /// </summary>
    public abstract class UsageCountedObject : IDisposable {

        private int usageCount;
        private bool disposedValue;
        private readonly ReaderWriterLockSlim lck
            = new(LockRecursionPolicy.NoRecursion);

        /// <summary>
        ///     pin this object
        /// </summary>
        public void Pin() {
            lck.EnterWriteLock();
            try {
                usageCount++;
                if (usageCount < 0)
                    throw new OverflowException();
            }
            finally {
                lck.ExitWriteLock();
            }
        }

        /// <summary>
        ///     unpin this object
        /// </summary>
        public async void UnPin(CancellationToken cancellationToken = default) {
            lck.EnterWriteLock();
            try {
                usageCount--;
                await DoUnPin(usageCount != 0, cancellationToken).ConfigureAwait(false);
                if (usageCount < 0)
                    throw new OverflowException();
            }
            finally {
                lck.ExitWriteLock();
            }
        }

        /// <summary>
        ///     action on unpin
        /// </summary>
        protected virtual ValueTask DoUnPin(bool isPinned, CancellationToken cancellationToken = default)
            => new();

        /// <summary>
        ///     check if this object is in use
        /// </summary>
        public bool IsPinned {
            get {
                lck.EnterReadLock();
                try {
                    return usageCount > 0;
                }
                finally {
                    lck.ExitReadLock();
                }
            }
        }

        /// <summary>
        ///     dispose this object
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (disposedValue)
                return;

            if (disposing)
                lck.Dispose();

            disposedValue = true;
        }

        /// <summary>
        ///     dispose this object
        /// </summary>
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
