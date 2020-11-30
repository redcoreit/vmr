using System.Collections.Generic;
using System.Linq;

using Vmr.Common.Instructions;

namespace Vmr.Common.Primitives
{
    internal class Method
    {
        public Method(int order, ProgramNode[] nodes, string name, int locals, byte args, bool isEntryPoint)
        {
            Order = order;
            Nodes = nodes;
            Name = name;
            Locals = locals;
            Args = args;
            IsEntryPoint = isEntryPoint;

            Size = nodes.Any()
                ? InstructionFacts.SizeOfMethodHeader + nodes.Select(m => m.Size).Aggregate((acc, curr) => acc + curr)
                : 0;
        }

        public int Size { get; }

        public int Order { get; }

        public IReadOnlyList<ProgramNode> Nodes { get; }

        public string Name { get; }

        public int Locals { get; }
        
        public byte Args { get; }
        
        public bool IsEntryPoint { get; }
    }
}
