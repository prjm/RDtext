namespace RDtext.Attributes {

    /// <summary>
    ///     immutable type
    /// </summary>
    [Immutable]
    public sealed class ImmutableAttribute : ObjectStateDescriptionAttribute {

        /// <summary>
        ///     create a new attribute instance
        /// </summary>
        public ImmutableAttribute() : base(true) {
        }
    }
}
