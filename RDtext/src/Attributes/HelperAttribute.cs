using System;

namespace RDtext.Attributes {

    /// <summary>
    ///     mark static helper classes
    /// </summary>
    [Immutable]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HelperAttribute : RDAttribute {

    }
}
