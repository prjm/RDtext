
using System;
using System.Globalization;
using System.Threading.Tasks;
using RDtext.Base;
using RDtext.Buffers;


[assembly: CLSCompliant(true)]

namespace RDtext.Example {
    public static class Program {

        public static async Task Main() {
            await using var buffers = new FileBuffers(new FileBufferOptions());
            var id = new BufferId(Guid.NewGuid(), "*");
            var buffer = await buffers.AddForFile(id, "d:\\temp\\2.abc").NoSync();

            for (var i = 0; i < buffer.PageCount; i++) {
                await using var p1 = UseAndReturn.That(await buffer.GetPageAsync(i).NoSync());
                Console.WriteLine("Data: " + p1.Data[0].ToString(CultureInfo.CurrentCulture));
            }
        }
    }
}
