
using System;
using System.Globalization;
using System.Threading.Tasks;

using RDtext.Buffers;
using RDtext.DataPooling;


[assembly: CLSCompliant(true)]

namespace RDtext.Example {
    public static class Program {

        public static async Task Main() {
            using var buffers = new FileBuffers();
            var id = new BufferId(Guid.NewGuid(), "*");
            var buffer = buffers.AddForFile(id, "d:\\temp\\2.abc");

            for (var i = 0; i < buffer.PageCount; i++) {
                using var p1 = UseAndReturn.That(await buffer.GetPageAsync(i).ConfigureAwait(false));
                Console.WriteLine("Data: " + p1.Data[0].ToString(CultureInfo.CurrentCulture));
            }
        }
    }
}
