using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Core.Abstractions
{
    internal static class InstructionFacts
    {
        public static bool TryGetInstructionCode(object value, [NotNullWhenAttribute(true)] out InstructionCode? instructionCode)
        {
            instructionCode = null;

            if(value is not int code)
            {
                return false;
            }

            instructionCode = (InstructionCode)code;
            return Enum.IsDefined<InstructionCode>(instructionCode.Value);
        }

        internal static string Format(InstructionCode instructionCode)
            => Format((int)instructionCode);

        internal static string Format(int instructionCode)
            => instructionCode.ToString("X2");
    }
}
