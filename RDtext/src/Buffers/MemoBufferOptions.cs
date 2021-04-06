using System.Collections.Generic;
using RDtext.Attributes;
using RDtext.Options;

namespace RDtext.Buffers {

    /// <summary>
    ///     options for memo buffers
    /// </summary>
    [Mutable]
    public class MemoBufferOptions : OptionSet<MemoBufferOptions> {

        /// <summary>
        ///     create a new set of memo buffer options
        /// </summary>
        /// <param name="parent"></param>
        public MemoBufferOptions(MemoBufferOptions? parent = default) : base("MemoBuffer", parent)
            => BufferOptions = new FileBufferOptions(parent?.BufferOptions);


        /// <summary>
        ///     buffer options
        /// </summary>
        public FileBufferOptions BufferOptions { get; }

        /// <summary>
        ///     all options
        /// </summary>
        protected override IEnumerable<OptionBase> Options {
            get {
                yield return BufferOptions;
            }
        }
    }
}
