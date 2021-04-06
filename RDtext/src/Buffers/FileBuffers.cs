using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RDtext.Base;
using RDtext.DataPooling;

namespace RDtext.Buffers {

    /// <summary>
    ///     page buffered manager
    /// </summary>
    public class FileBuffers : IAsyncDisposable {

        /// <summary>
        ///     default page size
        /// </summary>
        public const int DefaultPageSize
            = 100 * 1024;

        /// <summary>
        ///     default number of cached pages
        /// </summary>
        public const int DefaultNumberOfPages
            = 10;

        private readonly ConcurrentDictionary<BufferId, BufferBase> buffers
            = new();

        /// <summary>
        ///     used buffer pool
        /// </summary>
        public FixedSizeArrayPool<byte> Pool { get; }

        /// <summary>
        ///     create a new set of buffers
        /// </summary>
        /// <param name="options">buffer options</param>
        /// <param name="arrayPool"></param>
        public FileBuffers(FileBufferOptions options, FixedSizeArrayPool<byte>? arrayPool = default) {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            PageSize = options.PageSize.Value;
            NumberOfCachedPages = options.NumberOfPages.Value;
            Pool = arrayPool ?? new FixedSizeArrayPool<byte>(PageSize);

            if (NumberOfCachedPages < 0)
                throw new ArgumentOutOfRangeException(nameof(options), nameof(options.NumberOfPages));

            if (Pool.Capacity != PageSize)
                throw new ArgumentOutOfRangeException(nameof(options), nameof(options.PageSize));
        }

        /// <summary>
        ///     page size of this file buffer
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        ///     number of cached pages
        /// </summary>
        public int NumberOfCachedPages { get; }

        /// <summary>
        ///     add a new file buffer for a memory stream
        /// </summary>
        /// <returns></returns>
        public async ValueTask<BufferBase> AddForMememoryStream(BufferId id, MemoryStream s) {
            var buffer = new StreamBuffer(s, this);
            if (!buffers.TryAdd(id, buffer)) {
                await buffer.DisposeAsync().NoSync();
                throw new InvalidOperationException();
            }
            return buffer;
        }

        /// <summary>
        ///     add a new file buffer for a file path
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async ValueTask<BufferBase> AddForFile(BufferId id, string path) {
            var buffer = new FileBuffer(path, this);
            if (!buffers.TryAdd(id, buffer)) {
                await buffer.DisposeAsync().NoSync();
                throw new InvalidOperationException();
            }
            return buffer;
        }


        /// <summary>
        ///     dispose this buffer
        /// </summary>
        public virtual async ValueTask DisposeAsync() {
            var keys = new List<BufferId>(buffers.Keys);
            foreach (var key in keys)
                if (buffers.TryRemove(key, out var buffer))
                    await buffer.DisposeAsync().NoSync();
        }
    }
}
