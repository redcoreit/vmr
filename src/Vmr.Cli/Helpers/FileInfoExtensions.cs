using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Cli.Helpers
{
    public static class FileInfoExtensions
    {
        public static void ReadToMemory(this FileInfo fileInfo, MemoryStream memoryStream)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            using var stream = fileInfo.OpenRead();
            stream.CopyTo(memoryStream);
        }

        public static string GetContent(this FileInfo fileInfo)
        {
            using var ms = new MemoryStream();

            fileInfo.ReadToMemory(ms);
            var array = ms.ToArray();

            return Encoding.UTF8.GetString(array);
        }
    }
}
