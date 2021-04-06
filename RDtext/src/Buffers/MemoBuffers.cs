using System;
using System.Threading.Tasks;
using RDtext.Attributes;
using RDtext.Base;

namespace RDtext.Buffers {

    /// <summary>
    ///     buffer for char based i/o
    /// </summary>
    [Mutable]
    public class MemoBuffers : AsyncDisposable {

        private readonly FileBuffers buffers;

        /// <summary>
        ///     create a new memo buffer set
        /// </summary>
        /// <param name="options"></param>
        public MemoBuffers(MemoBufferOptions options) {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            buffers = new FileBuffers(options.BufferOptions);
        }

        /// <summary>
        ///     dispose this buffers
        /// </summary>
        /// <returns></returns>
        protected override async ValueTask DoDisposeAsync() {
            await buffers.DisposeAsync().NoSync();
            await base.DoDisposeAsync().NoSync();
        }
    }
}
