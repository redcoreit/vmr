using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Extensions.Configuration;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Helpers;
using Vmr.Cli.IO;
using Vmr.Cli.Options;
using Vmr.Runtime.Exceptions;
using Vmr.Runtime.Vm;

namespace Vmr.Cli.Commands
{
    internal class RunCommand : BaseCommand<RunOptions>
    {
        private readonly IFileReader _reader;
        private readonly IOutputWriter _writer;

        private RunCommand(IFileReader reader, IOutputWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        protected override string Name => "Run";

        internal static int Run(RunOptions opts, IFileReader reader, IOutputWriter writer, IConfiguration config) 
            => new RunCommand(reader, writer).Execute(opts, config);

        protected override void ExecuteInternal(RunOptions opts, IConfiguration config)
        {
            try
            {
                var program = _reader.ReadBinaryContent(opts.FilePath);
                var vm = new VirtualMachine();
                vm.Execute(program);
                var stack = vm.GetStack();

                if (stack.Any())
                {
                    foreach (var item in stack)
                    {
                        _writer.WriteLine(item?.ToString());
                    }
                }
                else
                {
                    _writer.WriteLine("Execution finished without result value.");
                }
            }
            catch (CliException ex)
            {
                _writer.WriteLine(ex.Message);
            }
            catch (VmExecutionException ex)
            {
                _writer.WriteLine(ex.Message);
            }
        }
    }
}