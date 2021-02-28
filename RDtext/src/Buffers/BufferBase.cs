using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RDtext.Buffers {

    /// <summary>
    ///     abstraction for a file buffer
    /// </summary>
    public abstract class BufferBase : IDisposable {

        private readonly Dictionary<long, Page> bufferedPages
            = new Dictionary<long, Page>();

        private readonly SemaphoreSlim mutex
            = new SemaphoreSlim(1, 1);

        private readonly Queue<long> pageQueue
            = new Queue<long>();

        /// <summary>
        ///     create a new file buffer
        /// </summary>
        /// <param name="fileBuffers"></param>
        protected BufferBase(FileBuffers fileBuffers)
            => Owner = fileBuffers ?? throw new ArgumentNullException(nameof(fileBuffers));

        /// <summary>
        ///     buffer owner
        /// </summary>
        public FileBuffers Owner { get; }

        /// <summary>
        ///     get the buffer length in bytes
        /// </summary>
        public abstract long Length { get; }

        /// <summary>
        ///     count of buffered pages
        /// </summary>
        public int NumberOfBufferedPages
            => bufferedPages.Count;

        /// <summary>
        ///     number of pages
        /// </summary>
        public long PageCount
            => (Length + Owner.PageSize - 1) / Owner.PageSize;

        /// <summary>
        ///     load a page into this buffer
        /// </summary>
        /// <param name="page">page to load</param>
        /// <param name="token">cancellation token</param>
        /// <param name="offset">offset of the page</param>
        /// <returns></returns>
        protected abstract ValueTask<int> LoadPageAsync(long offset, Page page, CancellationToken token = default);

        /// <summary>
        ///     get a page by number
        /// </summary>
        /// <param name="number">page number</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns></returns>
        public async ValueTask<Page> GetPageAsync(long number, CancellationToken cancellationToken = default) {

            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            await mutex.WaitAsync(cancellationToken).ConfigureAwait(false);
            try {
                if (bufferedPages.TryGetValue(number, out var result)) {
                    result.IncreaseUsageCount();
                    return result;
                }

                if (pageQueue.Count >= Owner.NumberOfCachedPages) {
                    var lastPage = pageQueue.Dequeue();
                    if (bufferedPages.TryGetValue(lastPage, out var oldPage) && oldPage.UsageCount < 1) {
                        bufferedPages.Remove(lastPage);
                        oldPage.ReturnBuffer();
                    }
                }

                result = new Page(number, this, Owner.GetPageBuffer());
                var offset = checked(Owner.PageSize * number);
                var loadPage = LoadPageAsync(offset, result, cancellationToken);
                if (loadPage.IsCompletedSuccessfully)
                    result.Length = loadPage.Result;
                else
                    result.Length = await loadPage.ConfigureAwait(false);

                bufferedPages.Add(number, result);
                pageQueue.Enqueue(number);
                return result;
            }
            finally {
                mutex.Release();
            }
        }

        /// <summary>
        ///     dispose this buffer
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (!disposing) return;

            var pages = new List<long>(bufferedPages.Count);
            foreach (var number in pages) {
                if (bufferedPages.Remove(number, out var page)) {
                    page.ReturnBuffer();
                }
            }
        }


        /// <summary>
        ///     dispose this object
        /// </summary>
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}