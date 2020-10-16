namespace Vmr.Common.Primitives
{
    internal sealed class Argument : ProgramNode
    {
        public Argument(uint size, object value)
        {
            Size = size;
            Value = value;
        }

        public override uint Size { get; }

        public object Value { get; }
    }
}
