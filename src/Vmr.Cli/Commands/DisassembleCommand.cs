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
using Vmr.Cli.Options;
using Vmr.Cli.Syntax;
using Vmr.Common.Disassemble;

namespace Vmr.Cli.Commands
{
    internal class DisassembleCommand : BaseCommand<DisassembleOptions>
    {
        private DisassembleCommand()
        {
        }

        protected override string Name => "Disassemble";

        internal static int Run(DisassembleOptions opts, IConfiguration config) => new DisassembleCommand().Execute(opts, config);

        protected override void ExecuteInternal(DisassembleOptions opts, IConfiguration config)
        {
            try
            {
                var file = new FileInfo(opts.FilePath);

                if (!file.Exists)
                {
                    throw new FileNotFoundException(opts.FilePath);
                }

                var content = file.GetBinaryContent();
                var program = Disassembler.GetProgram(content);
                var formatted = CodeFormatter.Format(program);

                WriteFileUtf8(file, formatted, opts.TargetFilePath);

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

        private void WriteFileUtf8(FileInfo file, string content, string? targetPath)
        {
            var filePath = targetPath is not null
                ? targetPath
                : Path.Combine(file.DirectoryName!, $"{Path.GetFileNameWithoutExtension(file.FullName)}.bin.vril");

            var directoryPath = Path.GetDirectoryName(filePath)!;

            if(!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(filePath, content, Encoding.UTF8);
        }
    }
}