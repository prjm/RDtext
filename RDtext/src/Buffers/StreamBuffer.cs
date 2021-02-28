using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RDtext.Buffers {

    /// <summary>
    ///     buffer for streams (for testing purposes)
    /// </summary>
    public class StreamBuffer : BufferBase {

        /// <summary>
        ///     create a new stream buffer
        /// </summary>
        /// <param name="stream">input stream</param>
        /// <param name="buffers">owner buffers</param>
        public StreamBuffer(Stream stream, FileBuffers buffers) : base(buffers)
            => Stream = stream;

        /// <summary>
        ///     stream length
        /// </summary>
        public override long Length
            => Stream.Length;

        /// <summary>
        ///     data stream
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        ///     load a page asynchronously
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="page"></param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns></returns>
        protected override async ValueTask<int> LoadPageAsync(long offset, Page page, CancellationToken cancellationToken) {
            Stream.Seek(offset, SeekOrigin.Begin);
            var result = Stream.ReadAsync(page.Data, cancellationToken);
            if (result.IsCompletedSuccessfully)
                return result.Result;
            else
                return await result.ConfigureAwait(false);
        }
    }
}
