using System;
using Microsoft.Extensions.Configuration;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Options;

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
                // TODO (RH -): implement
            }
            catch (CliException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}