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
        public const byte StringInitializer = (byte)'\'';
        public const byte StringTerminator = (byte)'\0';

        public static bool TryGetInstructionCode(object value, [NotNullWhen(true)] out InstructionCode? instructionCode)
        {
            instructionCode = null;

            if (!(value is int code))
                return false;

            instructionCode = (InstructionCode)code;
            return Enum.IsDefined(typeof(InstructionCode), instructionCode.Value);
        }

        public static string Format(InstructionCode instructionCode)
            => Format((int)instructionCode);

        public static string Format(int instructionCode)
            => instructionCode.ToString("X2");
    }
}
