namespace Vmr.Cli.IO
{
    public interface IOutputWriter
    {
        void Write(string? content);
        
        void WriteLine(string? content);
    }
}
