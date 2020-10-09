#pragma warning disable CA1812 //Never instantiate

using CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace Vmr.Cli.Options
{
    [Verb("dasm", HelpText = "Disassemble binary file to vmil file.")]
    internal class DisassembleOptions : BaseOptions
    {
        [NotNull]
        [Value(0, Required = true, MetaName = "file", HelpText = "Specify the bin file path.")]
        public string? FilePath { get; set; }

        [Option("target", Required = false, Hidden = false, HelpText = "Specify the output file path.")]
        public string? TargetFilePath { get; set; }
    }
}