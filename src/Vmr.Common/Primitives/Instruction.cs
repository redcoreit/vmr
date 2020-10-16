using Vmr.Common.Instructions;

namespace Vmr.Common.Primitives
{
    internal sealed class Instruction : ProgramNode
    {
        public Instruction(InstructionCode instructionCode)
        {
            InstructionCode = instructionCode;
        }

        public override uint Size => InstructionFacts.SizeOfOpCode;

        public InstructionCode InstructionCode { get; }
    }
}
