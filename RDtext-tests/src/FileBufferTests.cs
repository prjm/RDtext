using System;
using System.IO;
using System.Threading.Tasks;
using RDtext.Base;
using RDtext.Buffers;

namespace RDtext.Tests {


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
        public async Task TestPageCaching() {
            await using var buffers = new FileBuffers(10, 5);
            var memStream = new MemoryStream();

            var data = new byte[150];
            for (byte i = 0; i < data.Length; i++)
                data[i] = i;

            memStream.Write(data, 0, data.Length);

            var id = new BufferId(Guid.NewGuid(), "*");
            await using var buffer = await buffers.AddForMememoryStream(id, memStream).NoSync();
            await using var page1 = await buffer.GetPageAsync(0).ConfigureAwait(false);
            await using var page2 = await buffer.GetPageAsync(1).ConfigureAwait(false);
            await using var page3 = await buffer.GetPageAsync(2).ConfigureAwait(false);
            await using var page4 = await buffer.GetPageAsync(3).ConfigureAwait(false);
            await using (var page5 = await buffer.GetPageAsync(4).ConfigureAwait(false)) {

                AssertEqual(0L, page1.Number);
                AssertEqual(1L, page2.Number);
                AssertEqual(2L, page3.Number);
                AssertEqual(3L, page4.Number);
                AssertEqual(4L, page5.Number);

                AssertEqual(5, buffer.NumberOfBufferedPages);

                await using var page6 = await buffer.GetPageAsync(5).ConfigureAwait(false);
                AssertEqual(5L, page6.Number);
                AssertEqual(6, buffer.NumberOfBufferedPages);
                await page6.UnPin().NoSync();
            }
            AssertEqual(5, buffer.NumberOfBufferedPages);
        }

        [TestMethod]
        public async Task TestReadFromStream() {
            await using var buffers = new FileBuffers(100);
            using var memStream = new MemoryStream();

            var data = new byte[150];
            for (byte i = 0; i < data.Length; i++)
                data[i] = i;
            memStream.Write(data, 0, data.Length);

            var id = new BufferId(Guid.NewGuid(), "*");
            await using var buffer = await buffers.AddForMememoryStream(id, memStream).NoSync();
            var page = UseAndReturn.That(await buffer.GetPageAsync(0).ConfigureAwait(false));
            AssertEqual(1, page.Data[1]);
            AssertEqual(100, page.Data.Length);
            await page.DisposeAsync().NoSync();

            page = UseAndReturn.That(await buffer.GetPageAsync(0).ConfigureAwait(false));
            AssertEqual(0L, page.Data.Number);
            AssertEqual(1, buffer.NumberOfBufferedPages);
            await page.DisposeAsync().NoSync();
        }

    }
}
