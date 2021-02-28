using System;

namespace RDtext_tests {

    public class CommonTest {

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
