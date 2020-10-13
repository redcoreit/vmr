using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.IO;

namespace Vmr.Cli.Tests.Internal.Mock
{
    internal sealed class InterceptorFileReader : IFileReader
    {
        private object _content;

        public InterceptorFileReader(string content) 
            => _content = content;
        
        public InterceptorFileReader(byte[] content) 
            => _content = content;

        public byte[] ReadBinaryContent(string filePath) 
            => (byte[])_content;
        
        public string ReadStringContent(string filePath) 
            => (string)_content;
    }
}
