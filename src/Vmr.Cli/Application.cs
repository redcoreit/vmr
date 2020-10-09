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
using Vmr.Cli.Options;

namespace Vmr.Cli
{
    internal class Application
    {
        public static int Execute(string[] args, IConfiguration config)
            => Parser.Default.ParseArguments<RunOptions, AssembleOptions, DisassembleOptions>(args).MapResult(
                      (RunOptions opts) => RunCommand.Run(opts, config),
                      (AssembleOptions opts) => AssembleCommand.Run(opts, config),
                      (DisassembleOptions opts) => DisassembleCommand.Run(opts, config),
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
