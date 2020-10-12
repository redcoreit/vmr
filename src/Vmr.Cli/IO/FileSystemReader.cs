using System.IO;
using System.Text;

namespace Vmr.Cli.IO
{
    internal sealed class FileSystemReader : IFileReader
    {
        public string ReadStringContent(string filePath)
        {
            var file = new FileInfo(filePath);

            if (!file.Exists)
            {
                throw new FileNotFoundException(filePath);
            }

            var content = GetTextContent(file);
            return content;
        }

        public byte[] ReadBinaryContent(string filePath)
        {
            var file = new FileInfo(filePath);

            if (!file.Exists)
            {
                throw new FileNotFoundException(filePath);
            }

            var content = GetBinaryContent(file);
            return content;
        }

        private static void ReadToMemory(FileInfo fileInfo, MemoryStream memoryStream)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            using var stream = fileInfo.OpenRead();
            stream.CopyTo(memoryStream);
        }

        private static byte[] GetBinaryContent(FileInfo fileInfo)
        {
            using var ms = new MemoryStream();

            ReadToMemory(fileInfo, ms);
            var array = ms.ToArray();

            return array;
        }

        private static string GetTextContent(FileInfo fileInfo)
        {
            var array = GetBinaryContent(fileInfo);
            return Encoding.UTF8.GetString(array);
        }
    }
}
