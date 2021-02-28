using System;
using System.Threading;

namespace RDtext.DataPooling {

    /// <summary>
    ///     base class for pool items
    /// </summary>
    public abstract class PoolItem : IDisposable {

        private int rentCount = 0;

        /// <summary>
        ///     create a new pool item
        /// </summary>
        /// <param name="pool"></param>
        protected internal PoolItem(Pool pool) {
            ObjectPool = pool;
            rentCount = 1;
        }

        /// <summary>
        ///     link to the pool item owner
        /// </summary>
        public Pool ObjectPool { get; }

        /// <summary>
        ///     check if the item is currently in the pool
        /// </summary>
        protected internal bool IsInPool
            => rentCount == 0;

        /// <summary>
        ///     clear this item
        /// </summary>
        protected internal abstract void Clear();

        /// <summary>
        ///     increment rent count
        /// </summary>
        protected internal void Rent()
            => Interlocked.Increment(ref rentCount);

        /// <summary>
        ///     dispose this object
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (!disposing)
                return;

            ObjectPool.Return(this);
        }

        /// <summary>
        ///     return this item to the pool
        /// </summary>
        protected internal void Return()
            => Interlocked.Exchange(ref rentCount, 0);

        /// <summary>
        ///     dispose this object
        /// </summary>
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
