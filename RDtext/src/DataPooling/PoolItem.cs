using System.Threading;
using System.Threading.Tasks;

namespace RDtext.DataPooling {

    /// <summary>
    ///     base class for pool items
    /// </summary>
    public abstract class PoolItem : UsageCountedObject {

        /// <summary>
        ///     create a new pool item
        /// </summary>
        /// <param name="pool"></param>
        protected internal PoolItem(Pool pool)
            => ObjectPool = pool;

        /// <summary>
        ///     link to the pool item owner
        /// </summary>
        public Pool ObjectPool { get; }

        /// <summary>
        ///     clear this item
        /// </summary>
        protected internal abstract void Clear();

        /// <summary>
        ///     unpin this pool item
        /// </summary>
        /// <param name="isPinned"></param>
        /// <param name="cancellationToken">cancellation token</param>
        protected override ValueTask DoUnPin(bool isPinned, CancellationToken cancellationToken = default) {
            if (!isPinned) ObjectPool.ReturnPoolItem(this);
            return new ValueTask();
        }

    }
}
