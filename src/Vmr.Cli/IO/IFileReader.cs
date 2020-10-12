using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vmr.Cli.Helpers;

namespace Vmr.Cli.IO
{
    public interface IFileReader
    {
        string ReadStringContent(string filePath);

        byte[] ReadBinaryContent(string filePath);
    }
}
