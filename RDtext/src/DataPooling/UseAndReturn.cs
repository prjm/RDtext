using System;

namespace RDtext.DataPooling {

    /// <summary>
    ///     helper class for use and return objects
    /// </summary>
    public static class UseAndReturn {

        /// <summary>
        ///     helper method
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static UseAndReturn<T> That<T>(T element) where T : UsageCountedObject
            => new(element);

    }

    /// <summary>
    ///     helper structure for use and return scenarios
    /// </summary>
    public readonly struct UseAndReturn<T> : IDisposable, IEquatable<UseAndReturn<T>> where T : UsageCountedObject {

        /// <summary>
        ///     create a new use and return structure
        /// </summary>
        /// <param name="usedObject"></param>
        public UseAndReturn(T usedObject)
            => Data = usedObject ?? throw new ArgumentNullException(nameof(usedObject));

        /// <summary>
        ///     data
        /// </summary>
        public T Data { get; }

        /// <summary>
        ///     decrement usage count
        /// </summary>
        public void Dispose()
            => Data.UnPin();

        /// <summary>
        ///     check for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
            => obj is UseAndReturn<T> uar && Data.Equals(uar.Data);

        /// <summary>
        ///     check for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(UseAndReturn<T> other)
            => Data.Equals(other.Data);

        /// <summary>
        ///     get the hash code of the used object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => Data.GetHashCode();

        /// <summary>
        ///     check for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(UseAndReturn<T> left, UseAndReturn<T> right)
            => left.Equals(right);

        /// <summary>
        ///     check for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(UseAndReturn<T> left, UseAndReturn<T> right)
            => !(left == right);
    }
}
