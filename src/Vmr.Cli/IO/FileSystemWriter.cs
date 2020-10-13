using System.IO;
using System.Text;

namespace Vmr.Cli.IO
{
    internal sealed class FileSystemWriter : IFileWriter
    {
        public void WriteFile(string filePath, byte[] binary)
        {
            var file = new FileInfo(filePath);
            var binFileName = $"{Path.GetFileNameWithoutExtension(file.FullName)}.bin";
            var binFilePath = Path.Combine(file.DirectoryName!, binFileName);

            var directoryPath = Path.GetDirectoryName(filePath)!;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using var fs = new FileStream(binFilePath, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(fs);
            writer.Write(binary);
        }

        public void WriteFile(string filePath, string content)
        {
            var directoryPath = Path.GetDirectoryName(filePath)!;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(filePath, content, Encoding.UTF8);
        }
    }
}
