using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Helpers;
using Vmr.Cli.IO;
using Vmr.Cli.Options;
using Vmr.Cli.Workspace.Syntax;

namespace Vmr.Cli.Commands
{
    internal class AssembleCommand : BaseCommand<AssembleOptions>
    {
        private readonly IFileReader _reader;
        private readonly IFileWriter _writer;

        private AssembleCommand(IFileReader reader, IFileWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        protected override string Name => "Assemble";

        internal static int Run(AssembleOptions opts, IFileReader reader, IFileWriter writer, IConfiguration config) 
            => new AssembleCommand(reader, writer).Execute(opts, config);

        protected override void ExecuteInternal(AssembleOptions opts, IConfiguration config)
        {
            try
            {
                var content = _reader.ReadStringContent(opts.FilePath);
                var parser = new IlParser(content);
                var codeBuilder = parser.Parse();
                var program = codeBuilder.GetBinaryProgram();

                _writer.WriteFile(opts.FilePath, program);
            }
            catch (CliException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}