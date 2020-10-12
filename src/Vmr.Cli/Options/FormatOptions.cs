#pragma warning disable CA1812 //Never instantiate

using CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace Vmr.Cli.Options
{
    [Verb("format", HelpText = "Format and indent vril file.")]
    internal class FormatOptions : BaseOptions
    {
        [NotNull]
        [Value(0, Required = true, MetaName = "file", HelpText = "Specify the vmil file path.")]
        public string? FilePath { get; set; }
    }
}