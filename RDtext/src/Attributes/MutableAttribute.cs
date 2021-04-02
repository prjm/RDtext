namespace RDtext.Attributes {

    /// <summary>
    ///     mutable type
    /// </summary>
    [Immutable]
    public sealed class MutableAttribute : ObjectStateDescriptionAttribute {

        /// <summary>
        ///     create a new attribute
        /// </summary>
        public MutableAttribute() : base(false) {
        }
    }
}
