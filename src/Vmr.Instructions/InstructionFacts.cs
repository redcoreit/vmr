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

        public static bool TryCastInstructionCode(object value, [NotNullWhen(true)] out InstructionCode? instructionCode)
        {
            var result = value switch
            {
                InstructionCode code => Check(code, out instructionCode),
                int code => Check((InstructionCode)code, out instructionCode),
                null => throw new ArgumentNullException(nameof(value)),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };

            return result;

            static bool Check(InstructionCode instructionCode, out InstructionCode? result)
            {
                result = Enum.IsDefined(typeof(InstructionCode), instructionCode)
                    ? (InstructionCode?)instructionCode
                    : null;

                return result.HasValue;
            }
        }

        public static string Format(InstructionCode instructionCode)
            => Format((int)instructionCode);

        public static string Format(int instructionCode)
            => instructionCode.ToString("X2");
    }
}
