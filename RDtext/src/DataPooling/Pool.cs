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
        public abstract void Return(PoolItem poolItem);

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
            if (items.TryDequeue(out var result)) {
                result.Rent();
                return result;
            }

            return CreateItem();
        }

        /// <summary>
        ///     create a new pool item
        /// </summary>
        /// <returns></returns>
        protected abstract T CreateItem();

        /// <summary>
        ///     return a pool item
        /// </summary>
        /// <param name="item"></param>
        public override void Return(PoolItem item) {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (!ReferenceEquals(item.ObjectPool, this))
                throw new InvalidOperationException();

            if (item.IsInPool)
                return;

            lock (item) {
                if (item.IsInPool)
                    return;

                item.Return();
                item.Clear();
                items.Enqueue((T)item);
            }
        }

        /// <summary>
        ///     pool count
        /// </summary>
        public int Count
            => items.Count;

    }
}
