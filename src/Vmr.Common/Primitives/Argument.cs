namespace Vmr.Common.Primitives
{
    internal sealed class Argument : ProgramNode
    {
        public Argument(int size, object value)
        {
            Size = size;
            Value = value;
        }

        public override int Size { get; }

        public object Value { get; }
    }
}
