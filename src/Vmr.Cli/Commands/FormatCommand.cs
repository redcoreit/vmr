using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Helpers;
using Vmr.Cli.Options;
using Vmr.Cli.Workspace;
using Vmr.Cli.Workspace.Syntax;

namespace Vmr.Cli.Commands
{
    internal class FormatCommand : BaseCommand<FormatOptions>
    {
        private FormatCommand()
        {
        }

        protected override string Name => "Format";

        internal static int Run(FormatOptions opts, IConfiguration config) => new FormatCommand().Execute(opts, config);

        protected override void ExecuteInternal(FormatOptions opts, IConfiguration config)
        {
            try
            {
                var file = new FileInfo(opts.FilePath);

                if (!file.Exists)
                {
                    throw new FileNotFoundException(opts.FilePath);
                }

                var content = file.GetTextContent();                            
                var parser = new IlParser(content);
                var codeBuilder = parser.Parse();
                var program = codeBuilder.GetIlProgram();
                var code = CodeFormatter.Format(program);
                
                File.WriteAllText(file.FullName, code, Encoding.UTF8);
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