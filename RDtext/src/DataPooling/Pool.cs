using System;
using System.Collections.Concurrent;

namespace RDtext.DataPooling {

    /// <summary>
    ///     base class for object pools
    /// </summary>
    public abstract class Pool {

        /// <summary>
        ///     return the array pool item
        /// </summary>
        /// <param name="poolItem"></param>
        public abstract void ReturnPoolItem(PoolItem poolItem);

    }

    /// <summary>
    ///     base class for object pooling
    /// </summary>
    public abstract class Pool<T> : Pool where T : PoolItem {

        private readonly ConcurrentQueue<T> items
            = new ConcurrentQueue<T>();

        /// <summary>
        ///     rent a new pool item
        /// </summary>
        /// <returns></returns>
        public T Rent() {
            if (!items.TryDequeue(out var result)) {
                result = CreateItem();
            }

            result.Pin();
            return result;
        }

        /// <summary>
        ///     create a new pool item
        /// </summary>
        /// <returns></returns>
        protected abstract T CreateItem();

        /// <summary>
        ///     return a pool item
        /// </summary>
        /// <param name="poolItem"></param>
        public override void ReturnPoolItem(PoolItem poolItem) {
            if (poolItem is null)
                throw new ArgumentNullException(nameof(poolItem));

            if (!ReferenceEquals(poolItem.ObjectPool, this))
                throw new InvalidOperationException();

            poolItem.Clear();
            items.Enqueue((T)poolItem);
        }

        /// <summary>
        ///     pool count
        /// </summary>
        public int Count
            => items.Count;

    }
}
