using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Extensions.Configuration;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Helpers;
using Vmr.Cli.IO;
using Vmr.Cli.Options;
using Vmr.Cli.Workspace;
using Vmr.Common.Disassemble;

namespace Vmr.Cli.Commands
{
    internal class DisassembleCommand : BaseCommand<DisassembleOptions>
    {
        private readonly IFileReader _reader;
        private readonly IFileWriter _writer;

        private DisassembleCommand(IFileReader reader, IFileWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        protected override string Name => "Disassemble";

        internal static int Run(DisassembleOptions opts, IFileReader reader, IFileWriter writer, IConfiguration config)
            => new DisassembleCommand(reader, writer).Execute(opts, config);

        protected override void ExecuteInternal(DisassembleOptions opts, IConfiguration config)
        {
            try
            {
                var content = _reader.ReadBinaryContent(opts.FilePath);
                var program = Disassembler.GetProgram(content);
                var formatted = CodeFormatter.Format(program, new CodeFormatSettings() { UseIlRefPrefix = true });

                var path = GetTargetFilePath(opts.FilePath, opts.TargetFilePath);

                _writer.WriteFile(path, formatted);

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

        private string GetTargetFilePath(string filePath, string? targetFilePath)
        {
            var file = new FileInfo(filePath);
            var path = targetFilePath is not null
                ? targetFilePath
                : Path.Combine(file.DirectoryName!, $"{Path.GetFileNameWithoutExtension(file.FullName)}.dasm");

            return path;
        }
    }
}