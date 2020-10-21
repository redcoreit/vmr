namespace Vmr.Common.Primitives
{
    internal sealed class LabelDeclaration : ProgramNode
    {
        public LabelDeclaration(string name)
        {
            Name = name;
        }

        public override int Size => 0;
        
        public string Name { get; }
    }
}
