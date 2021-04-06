using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RDtext.Attributes;
using RDtext.Base;

namespace RDtext.Buffers {

    /// <summary>
    ///     abstraction for a file buffer
    /// </summary>
    [Mutable]
    public abstract class BufferBase : AsyncDisposable {

        private readonly Dictionary<long, Page> bufferedPages
            = new();

        private SemaphoreSlim? mutex
            = new(1, 1);

        private readonly Queue<long> pageQueue
            = new();

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
        ///     remove a page from the buffer
        /// </summary>
        /// <param name="page"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask RemovePageFromBuffer(Page page, CancellationToken cancellationToken = default) {

            if (page is null)
                throw new ArgumentNullException(nameof(page));

            ThrowIfObjectDisposed();

            await mutex!.WaitAsync(cancellationToken).ConfigureAwait(false);
            try {

                for (var i = 0; i < pageQueue.Count; i++) {
                    var item = pageQueue.Dequeue();
                    if (item != page.Number)
                        pageQueue.Enqueue(item);
                }

                if (bufferedPages.TryGetValue(page.Number, out var oldPage)) {
                    _ = bufferedPages.Remove(page.Number);
                    await oldPage.DisposeAsync().ConfigureAwait(false);
                }
            }
            finally {
                _ = mutex.Release();
            }
        }

        /// <summary>
        ///     get a page by number
        /// </summary>
        /// <param name="number">page number</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns></returns>
        public async ValueTask<Page> GetPageAsync(long number, CancellationToken cancellationToken = default) {

            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            ThrowIfObjectDisposed();

            await mutex!.WaitAsync(cancellationToken).NoSync();
            try {
                if (bufferedPages.TryGetValue(number, out var result)) {
                    result.Pin();
                    return result;
                }

                if (pageQueue.Count >= Owner.NumberOfCachedPages) {
                    var lastPage = pageQueue.Dequeue();
                    if (bufferedPages.TryGetValue(lastPage, out var oldPage) && !oldPage.IsPinned) {
                        _ = bufferedPages.Remove(lastPage);
                        await oldPage.DisposeAsync().ConfigureAwait(false);
                    }
                }

                result = new Page(number, this, Owner.Pool);
                result.Pin();
                var offset = checked(Owner.PageSize * number);
                var loadPage = LoadPageAsync(offset, result, cancellationToken);
                if (loadPage.IsCompletedSuccessfully)
                    result.Length = loadPage.Result;
                else
                    result.Length = await loadPage.NoSync();

                bufferedPages.Add(number, result);
                pageQueue.Enqueue(number);
                return result;
            }
            finally {
                _ = mutex.Release();
            }
        }

        /// <summary>
        ///     dispose this buffer
        /// </summary>
        protected override async ValueTask DoDisposeAsync() {

            if (mutex != default) {
                mutex.Dispose();
                mutex = default;
            }

            var pages = new List<long>(bufferedPages.Count);
            foreach (var number in pages) {
                if (bufferedPages.Remove(number, out var page)) {
                    await page.DisposeAsync().ConfigureAwait(false);
                }
            }

            await base.DoDisposeAsync().NoSync();
        }
    }
}
