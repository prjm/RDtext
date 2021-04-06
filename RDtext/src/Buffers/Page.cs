using System;
using System.Threading;
using System.Threading.Tasks;
using RDtext.Attributes;
using RDtext.Base;
using RDtext.DataPooling;

namespace RDtext.Buffers {

    /// <summary>
    ///     page definition
    /// </summary>
    [Mutable]
    public class Page : UsageCountedObject {

        /// <summary>
        ///     create a new page
        /// </summary>
        /// <param name="number">page number</param>
        /// <param name="owner">buffer owner</param>
        /// <param name="buffer">data buffer</param>
        internal Page(long number, BufferBase owner, FixedSizeArrayPool<byte> buffer) {
            Number = number;
            Owner = owner;
            Buffer = buffer.Rent();
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
        ///     data length of this page
        /// </summary>
        public int Length { get; internal set; }

        /// <summary>
        ///     get the data array of this buffer
        /// </summary>
        /// <returns></returns>
        public byte[] GetBuffer()
            => (Buffer ?? throw new ObjectDisposedException(nameof(Buffer))).GetData();

        /// <summary>
        ///     byte by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
            => (Buffer ?? throw new ObjectDisposedException(nameof(Buffer)))[index];


        /// <summary>
        ///     unpin this page
        /// </summary>
        /// <param name="isPinned"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async ValueTask DoUnPin(bool isPinned, CancellationToken cancellationToken) {
            if (!isPinned)
                await Owner.RemovePageFromBuffer(this, cancellationToken).NoSync();
        }

        /// <summary>
        ///     dispose this page and return the buffer
        /// </summary>
        /// <returns></returns>
        protected override async ValueTask DoDisposeAsync() {
            await ReturnBuffer().NoSync();
            await base.DoDisposeAsync().NoSync();
        }

        private async ValueTask ReturnBuffer() {
            var buf = Buffer;
            Buffer = default;
            if (buf != default)
                await buf.UnPin().NoSync();
        }
    }
}
