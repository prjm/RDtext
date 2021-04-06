using RDtext.Attributes;

namespace RDtext.Options {

    /// <summary>
    ///     integer option
    /// </summary>
    [Mutable]
    public class IntegerOption : OptionBase<int> {

        /// <summary>
        ///     create a new integer option
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="defaultValue"></param>
        public IntegerOption(string name, OptionBase<int>? parent, int defaultValue) : base(name, parent, defaultValue) {
        }

    }
}
