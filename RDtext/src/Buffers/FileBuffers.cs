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

        private const int DefaultPageSize
            = 100 * 1024;

        private const int DefaultNumberOfPages
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
        /// <param name="pageSize">page size</param>
        /// <param name="numberOfPages">number of cached read pages</param>
        /// <param name="arrayPool"></param>
        public FileBuffers(int pageSize = DefaultPageSize, int numberOfPages = DefaultNumberOfPages, FixedSizeArrayPool<byte>? arrayPool = default) {
            PageSize = pageSize;
            NumberOfCachedPages = numberOfPages;
            Pool = arrayPool ?? new FixedSizeArrayPool<byte>(pageSize);

            if (numberOfPages < 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfPages));

            if (Pool.Capacity != pageSize)
                throw new ArgumentOutOfRangeException(nameof(pageSize));
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
