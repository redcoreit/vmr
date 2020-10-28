namespace Vmr.Cli.Workspace
{
    internal record CodeFormatSettings
    {
        public bool UseIlRefPrefix { get; init; } = false;
        
        public int InstructionIndentSize { get; init; } = 2;
        
        public int LineIndentSize { get; init; } = 4;
    }
}
