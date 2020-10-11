using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Common.Instructions
{
    public static class InstructionFacts
    {
        public const byte Eos = (byte)'\0';
        public const int SizeOfEos = sizeof(char);
        public const int SizeOfOpCode = sizeof(InstructionCode);

        public static string Format(InstructionCode instructionCode)
            => Format((int)instructionCode);

        public static string Format(int instructionCode)
            => $"0x{instructionCode.ToString("X2")}";

        public static int GetArgumentsCount(InstructionCode instructionCode)
            => instructionCode switch
            {
                InstructionCode.Add => 0,
                InstructionCode.Ldc_i4 => 1,
                InstructionCode.Ldstr => 1,
                InstructionCode.Pop => 0,
                InstructionCode.Br => 1,
                InstructionCode.Nop => 0,
                InstructionCode.Ceq => 0,
                InstructionCode.Brfalse => 1,
                InstructionCode.Brtrue => 1,
                InstructionCode.Ldloc => 1,
                InstructionCode.Stloc => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(instructionCode), instructionCode, null)
            };

        public static bool IsBranchingInstruction(InstructionCode instructionCode)
            => instructionCode switch
            {
                InstructionCode.Br => true,
                InstructionCode.Brfalse => true,
                InstructionCode.Brtrue => true,
                _ => false,
            };
    }
}
