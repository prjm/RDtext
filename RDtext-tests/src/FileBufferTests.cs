using System;
using System.IO;

using RDtext.Buffers;

namespace RDtext_tests {


    public class FileBufferTests : CommonTest {

        [TestMethod]
        public void TestBufferId() {
            var g = Guid.NewGuid();
            var b1 = new BufferId(g, "x");
            var b2 = new BufferId(g, "y");
            var b3 = new BufferId(Guid.NewGuid(), "x");

            AssertEqual(b1, b2);
            AssertEqual(b1.GetHashCode(), b2.GetHashCode());
            AssertEqual(false, b1.Equals(b3));
        }

        [TestMethod]
        public async void TestReadFromStream() {
            var buffers = new FileBuffers(100);
            var memStream = new MemoryStream();

            var data = new byte[150];
            for (byte i = 0; i < data.Length; i++)
                data[i] = i;

            memStream.Write(data, 0, data.Length);

            var id = new BufferId(Guid.NewGuid(), "*");
            var buffer = buffers.AddForMememoryStream(id, memStream);
            var page = await buffer.GetPageAsync(0).ConfigureAwait(false);
            AssertEqual(1, page[1]);
            AssertEqual(100, page.Length);
            page.Dispose();

            page = await buffer.GetPageAsync(0).ConfigureAwait(false);
            AssertEqual(0L, page.Number);
            AssertEqual(1, buffer.NumberOfBufferedPages);
        }

    }
}
