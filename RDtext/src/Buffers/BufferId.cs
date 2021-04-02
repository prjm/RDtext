using System;
using RDtext.Attributes;

namespace RDtext.Buffers {

    /// <summary>
    ///     buffer identifier
    /// </summary>
    [Immutable]
    public class BufferId : IEquatable<BufferId> {

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        public BufferId(Guid id, string label) {
            Id = id;
            Label = label;
        }

        /// <summary>
        ///     buffer id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        ///     buffer label
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     check for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BufferId? other)
            => other?.Id.Equals(Id) ?? false;

        /// <summary>
        ///     check for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
            => obj is BufferId id && Equals(id);

        /// <summary>
        ///     hash code of the GUID
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => Id.GetHashCode();
    }
}
