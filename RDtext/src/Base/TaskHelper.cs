using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RDtext.Attributes;

namespace RDtext.Base {

    /// <summary>
    ///     helper class for value tasks
    /// </summary>
    [Helper]
    public static class TaskHelper {

        /// <summary>
        ///     run awaitables out of the synchronization context
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredTaskAwaitable NoSync(this Task task) {
            if (task is null)
                throw new ArgumentNullException(nameof(task));

            return task.ConfigureAwait(false);
        }

        /// <summary>
        ///     run awaitables out of the synchronization context
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredTaskAwaitable<T> NoSync<T>(this Task<T> task) {
            if (task is null)
                throw new ArgumentNullException(nameof(task));

            return task.ConfigureAwait(false);
        }
    }
}
