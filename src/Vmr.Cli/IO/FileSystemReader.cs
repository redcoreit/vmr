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

        private static byte[] GetBinaryContent(FileInfo fileInfo)
        {
            using var ms = new MemoryStream();
            using var stream = fileInfo.OpenRead();
            stream.CopyTo(ms);
            var array = ms.ToArray();

            return array;
        }

        private static string GetTextContent(FileInfo fileInfo)
        {
            using var stream = fileInfo.OpenRead();
            using var reader = new StreamReader(stream, new UTF8Encoding(false), true);

            return reader.ReadToEnd();
        }
    }
}
