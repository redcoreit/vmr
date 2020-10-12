using System.IO;
using System.Text;

namespace Vmr.Cli.IO
{
    internal sealed class FileSystemWriter : IFileWriter
    {
        public void WriteFile(string filepath, byte[] binary)
        {
            var file = new FileInfo(filepath);
            var binFileName = $"{Path.GetFileNameWithoutExtension(file.FullName)}.bin";
            var binFilePath = Path.Combine(file.DirectoryName!, binFileName);

            using var fs = new FileStream(binFilePath, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(fs);
            writer.Write(binary);
        }

        public void WriteFile(string filepath, string content, string? targetFilePath)
        {
            var file = new FileInfo(filepath);
            var filePath = targetFilePath is not null
                ? targetFilePath
                : Path.Combine(file.DirectoryName!, $"{Path.GetFileNameWithoutExtension(file.FullName)}.bin.vril");

            var directoryPath = Path.GetDirectoryName(filePath)!;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(filePath, content, Encoding.UTF8);
        }
    }
}
