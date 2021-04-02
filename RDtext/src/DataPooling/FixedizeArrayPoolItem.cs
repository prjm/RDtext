using System;
using RDtext.Attributes;

namespace RDtext.DataPooling {

    /// <summary>
    ///     fixed size array pool item
    /// </summary>
    /// <typeparam name="T">base data type</typeparam>
    [Mutable]
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

            data = new T[capacity];
            Capacity = capacity;
        }

        /// <summary>
        ///     array data
        /// </summary>
        private readonly T[] data;

        /// <summary>
        ///     access array item
        /// </summary>
        /// <param name="index">item index</param>
        /// <returns></returns>
        public T this[int index]
            => data[index];

        /// <summary>
        ///     array capacity
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        ///     clear the array
        /// </summary>
        protected internal override void Clear() {

            if (data.Length != Capacity)
                throw new InvalidOperationException();

            Array.Clear(data, 0, data.Length);
        }


        internal T[] GetData() => data;
    }
}
