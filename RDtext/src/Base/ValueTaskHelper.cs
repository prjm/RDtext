using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RDtext.Attributes;

namespace RDtext.Base {

    /// <summary>
    ///     helper class for value tasks
    /// </summary>
    [Helper]
    public static class ValueTaskHelper {

        /// <summary>
        ///     run awaitables out of the synchronization context
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredValueTaskAwaitable NoSync(this ValueTask task)
            => task.ConfigureAwait(false);

        /// <summary>
        ///     run awaitables out of the synchronization context
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredValueTaskAwaitable<T> NoSync<T>(this ValueTask<T> task)
            => task.ConfigureAwait(false);


    }
}
