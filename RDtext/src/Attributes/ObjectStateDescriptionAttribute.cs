using System;

namespace RDtext.Attributes {

    /// <summary>
    ///     object state description attribute
    /// </summary>
    [Immutable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public abstract class ObjectStateDescriptionAttribute : RDAttribute {

        /// <summary>
        ///     create a new attribute instance
        /// </summary>
        /// <param name="isImmutable"></param>
        protected ObjectStateDescriptionAttribute(bool isImmutable)
            => IsImmutable = isImmutable;

        /// <summary>
        ///     check if this type is immutable
        /// </summary>
        public bool IsImmutable { get; }
    }
}
