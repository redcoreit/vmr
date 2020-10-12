using System;

namespace Vmr.Cli.IO
{
    internal sealed class ConsoleWriter : IOutputWriter
    {
        public void Write(string? content) 
            => Console.Write(content);

        public void WriteLine(string? content)
            => Console.WriteLine(content);
    }
}
