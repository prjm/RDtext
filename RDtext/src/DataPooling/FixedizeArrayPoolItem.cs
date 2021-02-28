using System;

namespace RDtext.DataPooling {

    /// <summary>
    ///     fixed size array pool item
    /// </summary>
    /// <typeparam name="T">base data type</typeparam>
    public class FixedizeArrayPoolItem<T> : PoolItem {

        /// <summary>
        ///     create a new array pool item
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="capacity"></param>
        public FixedizeArrayPoolItem(Pool pool, int capacity) : base(pool) {
            if (pool is null)
                throw new ArgumentNullException(nameof(pool));

            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            Data = new T[capacity];
            Capacity = capacity;
        }

        /// <summary>
        ///     array data
        /// </summary>
        public T[] Data { get; }

        /// <summary>
        ///     array capacity
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        ///     clear the array
        /// </summary>
        protected internal override void Clear() {

            if (Data.Length != Capacity)
                throw new InvalidOperationException();

            Array.Clear(Data, 0, Data.Length);
        }
    }
}