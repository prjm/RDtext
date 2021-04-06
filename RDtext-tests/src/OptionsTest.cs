using System.Collections.Generic;
using RDtext.Options;

namespace RDtext.Tests {

    public class TestOptions : OptionSet<TestOptions> {
        public TestOptions(TestOptions? parent = default) : base("test", parent)
            => A = new IntegerOption("A", parent?.A, 5);

        protected override IEnumerable<OptionBase> Options {
            get {
                yield return A;
            }
        }

        public IntegerOption A { get; }
    }

    /// <summary>
    ///     test options
    /// </summary>
    public class OptionsTest : CommonTest {

        [TestMethod]
        public void TestBasics() {
            var o = new TestOptions();
            AssertEqual("test", o.Name);
            AssertEqual(1, o.AllOptions.Count);
            AssertEqual("A", o.AllOptions[0].Name);
            AssertEqual("A", o.GetOptionByName("A")?.Name);
        }

        [TestMethod]
        public void TestIntegerOption() {
            var p = new TestOptions();
            var o = new TestOptions(p);

            AssertEqual(5, o.A.Value);
            AssertEqual(5, p.A.Value);

            p.A.Value = 10;
            AssertEqual(10, o.A.Value);
            AssertEqual(10, p.A.Value);
            AssertEqual(true, p.A.IsSet);
            AssertEqual(false, o.A.IsSet);

            p.A.Reset();
            AssertEqual(5, o.A.Value);
            AssertEqual(5, p.A.Value);
            AssertEqual(false, p.A.IsSet);
            AssertEqual(false, o.A.IsSet);
        }

    }
}
