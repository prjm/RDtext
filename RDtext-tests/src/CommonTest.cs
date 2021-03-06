using System;

namespace RDtext.Tests {

    /// <summary>
    ///     base class for tests
    /// </summary>
    public abstract class CommonTest {

        public static void AssertEqual(int expected, int actual)
            => Xunit.Assert.Equal(expected, actual);

        public static void AssertEqual(bool expected, bool actual)
            => Xunit.Assert.Equal(expected, actual);

        public static void AssertEqual<T>(T expected, T actual)
            => Xunit.Assert.Equal(expected, actual);

        public static void AssertError<T>(Action action) where T : Exception
            => Xunit.Assert.Throws<T>(action);

    }
}
