using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Helpers;
using Vmr.Cli.IO;
using Vmr.Cli.Options;
using Vmr.Cli.Workspace;
using Vmr.Cli.Workspace.Syntax;

namespace Vmr.Cli.Commands
{
    internal class FormatCommand : BaseCommand<FormatOptions>
    {
        private readonly IFileReader _reader;
        private readonly IFileWriter _writer;

        private FormatCommand(IFileReader reader, IFileWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        protected override string Name => "Format";

        internal static int Run(FormatOptions opts, IFileReader reader, IFileWriter writer, IConfiguration config)
            => new FormatCommand(reader, writer).Execute(opts, config);

        protected override void ExecuteInternal(FormatOptions opts, IConfiguration config)
        {
            try
            {
                var content = _reader.ReadStringContent(opts.FilePath);
                var parser = new IlParser(content);
                var codeBuilder = parser.Parse();
                var program = codeBuilder.GetIlProgram();
                var code = CodeFormatter.Format(program);

                _writer.WriteFile(opts.FilePath, code, null);
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