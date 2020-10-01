#pragma warning disable CA1812 //Never instantiate

using CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace Vmr.Cli.Options
{
    [Verb("asm", HelpText = "Assemble binary file from vmil file.")]
    internal class AssembleOptions : BaseOptions
    {
        [NotNull]
        [Value(0, Required = true, MetaName = "file", HelpText = "Specify the vmil file path.")]
        public string? FilePath { get; set; }
    }
}