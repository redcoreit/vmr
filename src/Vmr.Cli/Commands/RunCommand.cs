using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Extensions.Configuration;
using Vmr.Assembler;
using Vmr.Assembler.Exceptions;
using Vmr.Cli.Options;
using Vmr.Core;
using Vmr.Core.Exceptions;

namespace Vmr.Cli.Commands
{
    internal class RunCommand : BaseCommand<RunOptions>
    {
        private RunCommand()
        {
        }

        protected override string Name => "Run";

        internal static int Run(RunOptions opts, IConfiguration config) => new RunCommand().Execute(opts, config);

        protected override void ExecuteInternal(RunOptions opts, IConfiguration config)
        {
            try
            {
                var file = new FileInfo(opts.FilePath);

                if(!file.Exists)
                {
                    throw new FileNotFoundException(opts.FilePath);
                }

                var program = SimpleHumanReadableFileProcessor.Process(file);
                var vm = new VirtualMachine(program);
                vm.Execute(Enumerable.Empty<object>());
                var result = vm.GetStack().SingleOrDefault();

                if(result is not null)
                {
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine("Execution finished without result value.");
                }
            }
            catch (AssemblerException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (VmExecutionException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}