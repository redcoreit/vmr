using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Helpers;
using Vmr.Cli.Options;
using Vmr.Cli.Syntax;
using Vmr.Instructions;

namespace Vmr.Cli.Commands
{
    internal class AssembleCommand : BaseCommand<AssembleOptions>
    {
        private AssembleCommand()
        {
        }

        protected override string Name => "Assemble";

        internal static int Run(AssembleOptions opts, IConfiguration config) => new AssembleCommand().Execute(opts, config);

        protected override void ExecuteInternal(AssembleOptions opts, IConfiguration config)
        {
            try
            {
                var file = new FileInfo(opts.FilePath);

                if (!file.Exists)
                {
                    throw new FileNotFoundException(opts.FilePath);
                }

                var content = file.GetContent();                
                var parser = new IlParser(content);
                var codeBuilder = parser.Parse();
                var program = codeBuilder.Compile();
                WriteBinaryFile(file, program);
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

        private void WriteBinaryFile(FileInfo file, byte[] binary)
        {
            var binFileName = $"{Path.GetFileNameWithoutExtension(file.FullName)}.bin";
            var binFilePath = Path.Combine(file.DirectoryName!, binFileName);

            using var fs = new FileStream(binFilePath, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(fs);
            writer.Write(binary);
        }
    }
}