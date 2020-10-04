using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Instructions
{
    public static class InstructionFacts
    {
        public const byte StringTerminator = (byte)'\0';

        public static string Format(InstructionCode instructionCode)
            => Format((int)instructionCode);

        public static string Format(int instructionCode)
            => instructionCode.ToString("X2");
    }
}
