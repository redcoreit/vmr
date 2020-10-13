using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.IO;

namespace Vmr.Cli.Tests.Internal.Mock
{
    internal class InterceptorFileWriter : IFileWriter
    {
        private readonly Dictionary<string, object> _files;

        public InterceptorFileWriter()
        {
            _files = new Dictionary<string, object>();
        }

        public IReadOnlyDictionary<string, object> Files => _files;

        public void WriteFile(string filePath, byte[] binary)
            => _files[filePath] = binary;

        public void WriteFile(string filePath, string content)
            => _files[filePath] = content;
    }
}
