
using System;
using System.Threading.Tasks;

using RDtext.Buffers;

namespace RDtext.Example {
    public class Program {

        public static async Task Main() {
            using var buffers = new FileBuffers();
            var id = new BufferId(Guid.NewGuid(), "*");
            var buffer = buffers.AddForFile(id, "d:\\temp\\2.abc");

            for (var i = 0; i < buffer.PageCount; i++) {
                var p1 = await buffer.GetPageAsync(i);
                Console.WriteLine("Data: " + p1[0].ToString());
                p1.Dispose();
            }
        }
    }
}
