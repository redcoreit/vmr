using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.IO;

namespace Vmr.Cli.Tests.Internal.Mock
{
    internal sealed class InterceptiorOutputWriter : IOutputWriter
    {
        private readonly StringBuilder _builder;

        public InterceptiorOutputWriter()
        {
            _builder = new StringBuilder();
        }

        public string Output => _builder.ToString();

        public void Write(string? content)
            => _builder.Append(content);

        public void WriteLine(string? content) 
            => _builder.AppendLine(content);
    }
}
