using System;

namespace RDtext.DataPooling {

    /// <summary>
    ///     array pool
    /// </summary>
    public class FixedSizeArrayPool<T> : Pool<FixedizeArrayPoolItem<T>> {

        /// <summary>
        ///     create a new array pool
        /// </summary>
        /// <param name="arrayCapacity"></param>
        public FixedSizeArrayPool(int arrayCapacity) {

            if (arrayCapacity < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayCapacity));

            Capacity = arrayCapacity;
        }

        /// <summary>
        ///     array capacity
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        ///     create a new array pool item
        /// </summary>
        /// <returns>create a new array pool item</returns>
        protected override FixedizeArrayPoolItem<T> CreateItem()
            => new FixedizeArrayPoolItem<T>(this, Capacity);
    }
}
