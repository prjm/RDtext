using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RDtext.Buffers {

    /// <summary>
    ///     buffer for files
    /// </summary>
    public class FileBuffer : BufferBase {
        private readonly string filePath;

        /// <summary>
        ///     create a new file buffer
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileBuffers"></param>
        public FileBuffer(string path, FileBuffers fileBuffers) : base(fileBuffers)
            => filePath = path;

        /// <summary>
        ///     file length
        /// </summary>
        public override long Length
            => new FileInfo(filePath).Length;

        /// <summary>
        ///     load a page asynchronously
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="page"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected override async ValueTask<int> LoadPageAsync(long offset, Page page, CancellationToken token = default) {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, Owner.PageSize, true);
            stream.Seek(offset, SeekOrigin.Begin);
            return await stream.ReadAsync(page.Data, token).ConfigureAwait(false);
        }
    }
}