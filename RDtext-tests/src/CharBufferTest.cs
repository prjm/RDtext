using System.Threading.Tasks;

namespace RDtext.Tests {

    /// <summary>
    ///     char buffer tests
    /// </summary>
    public class CharBufferTest : CommonTest {

        [TestMethod]
        public async Task TestReadFromStreamAsync() {
            /*
            await using var buffers = new CharBuffers();
            var id = new BufferId(Guid.NewGuid(), "*");
            await using var buffer = await buffers.AddForMememoryStream(id, memStream).NoSync();
            var page = UseAndReturn.That(await buffer.GetPageAsync(0).ConfigureAwait(false));
            AssertEqual(1, page.Data[1]);
            AssertEqual(100, page.Data.Length);
            await page.DisposeAsync().NoSync();
            AssertEqual(1, buffers.Pool.Count);
            */
        }

    }
}
