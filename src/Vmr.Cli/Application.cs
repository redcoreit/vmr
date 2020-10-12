using CommandLine;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.Commands;
using Vmr.Cli.Helpers;
using Vmr.Cli.IO;
using Vmr.Cli.Options;

namespace Vmr.Cli
{
    internal class Application
    {
        private static readonly IFileReader _fileReader;
        private static readonly IFileWriter _fileWriter;
        private static readonly IOutputWriter _outputWriter;

        static Application()
        {
            _fileReader = new FileSystemReader();
            _fileWriter = new FileSystemWriter();
            _outputWriter = new ConsoleWriter();
        }

        public static int Execute(string[] args, IConfiguration config)
            => Parser.Default.ParseArguments<RunOptions, AssembleOptions, DisassembleOptions, FormatOptions>(args).MapResult(
                      (RunOptions opts) => RunCommand.Run(opts, _fileReader, _outputWriter, config),
                      (AssembleOptions opts) => AssembleCommand.Run(opts, _fileReader, _fileWriter, config),
                      (DisassembleOptions opts) => DisassembleCommand.Run(opts, _fileReader, _fileWriter, config),
                      (FormatOptions opts) => FormatCommand.Run(opts, _fileReader, _fileWriter, config),
                      errs => 1);

        public static int ReportCrash(Exception ex)
        {
            if (ex is null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            var builder = new StringBuilder();

            builder.AppendLine("Writing error dumps...");
            builder.AppendLine($"Executed command: {Environment.CommandLine}");
            builder.AppendLine();

            foreach (var item in ex.UnwrapReverse())
            {
                builder.AppendLine(item.GetType().FullName);
                builder.AppendLine(item.Message);
                builder.AppendLine(item.StackTrace);
            }

            Console.WriteLine(builder.ToString());

            return 1;
        }
    }
}
