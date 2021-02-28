using System;
using System.Threading;

using RDtext.DataPooling;

namespace RDtext.Buffers {

    /// <summary>
    ///     page definition
    /// </summary>
    public class Page : IDisposable {

        /// <summary>
        ///     create a new page
        /// </summary>
        /// <param name="number"></param>
        /// <param name="owner"></param>
        /// <param name="arrayPoolItem"></param>
        internal Page(long number, BufferBase owner, FixedizeArrayPoolItem<byte> arrayPoolItem) {
            Number = number;
            Owner = owner;
            Buffer = arrayPoolItem;
            usageCount = 1;
        }

        /// <summary>
        ///     page number
        /// </summary>
        public long Number { get; }

        /// <summary>
        ///     owner file
        /// </summary>
        internal BufferBase Owner { get; }

        /// <summary>
        ///     page buffer
        /// </summary>
        private FixedizeArrayPoolItem<byte>? Buffer { get; set; }

        /// <summary>
        ///     page usage count
        /// </summary>
        private int usageCount;

        /// <summary>
        ///     data length of this page
        /// </summary>
        public int Length { get; internal set; }

        /// <summary>
        ///     usage count
        /// </summary>
        public int UsageCount
            => usageCount;

        /// <summary>
        ///     page data
        /// </summary>
        public byte[] Data
            => (Buffer ?? throw new InvalidOperationException()).Data;

        /// <summary>
        ///     byte by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
            => (Buffer ?? throw new InvalidOperationException()).Data[index];

        internal void IncreaseUsageCount()
            => Interlocked.Increment(ref usageCount);

        /// <summary>
        ///     dispose this page
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (!disposing) return;
            Interlocked.Decrement(ref usageCount);
        }

        /// <summary>
        ///     dispose this object
        /// </summary>
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void ReturnBuffer() {
            var buf = Buffer;
            Buffer = default;
            if (buf != default)
                buf.Dispose();
        }
    }
}
