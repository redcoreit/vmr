using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Extensions.Configuration;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Helpers;
using Vmr.Cli.Options;
using Vmr.Runtime.Exceptions;
using Vmr.Runtime.Vm;

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
            catch (CliException ex)
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