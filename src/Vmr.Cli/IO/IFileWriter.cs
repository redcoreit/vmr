using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vmr.Cli.IO
{
    public interface IFileWriter
    {
        void WriteFile(string filePath, byte[] binary);

        void WriteFile(string filePath, string content);
    }
}
