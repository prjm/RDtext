using System.Collections.Generic;
using RDtext.Options;

namespace RDtext.Buffers {

    /// <summary>
    ///     options for file buffers
    /// </summary>
    public class FileBufferOptions : OptionSet<FileBufferOptions> {

        /// <summary>
        ///     create a new set of file buffer options
        /// </summary>
        /// <param name="parent"></param>
        public FileBufferOptions(FileBufferOptions? parent = default) : base("FileBuffer", parent) {
            PageSize = new IntegerOption("PageSize", parent?.PageSize, FileBuffers.DefaultPageSize);
            NumberOfPages = new IntegerOption("NumberOfPages", parent?.NumberOfPages, FileBuffers.DefaultNumberOfPages);
        }

        /// <summary>
        ///     create a new set of options
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="numberOfPages"></param>
        public FileBufferOptions(int pageSize, int numberOfPages) : this() {
            PageSize.Value = pageSize;
            NumberOfPages.Value = numberOfPages;
        }

        /// <summary>
        ///     page size
        /// </summary>
        public IntegerOption PageSize { get; }

        /// <summary>
        ///     number of pages
        /// </summary>
        public IntegerOption NumberOfPages { get; }

        /// <summary>
        ///     all options
        /// </summary>
        protected override IEnumerable<OptionBase> Options {
            get {
                yield return PageSize;
                yield return NumberOfPages;
            }
        }
    }
}
