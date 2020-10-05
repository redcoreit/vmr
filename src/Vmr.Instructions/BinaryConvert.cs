using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Vmr.Instructions
{
    public static class BinaryConvert
    {
        public static byte[] GetBytes(InstructionCode instruction)
            => new[] { (byte)instruction };

        public static byte[] GetBytes(string text)
        {
            var result = Encoding.UTF8.GetBytes(text).ToList();
            result.Add(InstructionFacts.StringTerminator);

            return result.ToArray();
        }

        public static byte[] GetBytes(int value)
        {
            var result = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        public static string GetString(ref int _pointer, ReadOnlySpan<byte> instructions)
        {
            if (instructions[_pointer] == InstructionFacts.StringTerminator)
            {
                return string.Empty;
            }

            var result = new List<byte>();

            do
            {
                result.Add(instructions[_pointer]);
            } while (_pointer++ < instructions.Length && instructions[_pointer] != InstructionFacts.StringTerminator);

            if (instructions[_pointer] != InstructionFacts.StringTerminator)
                throw new InvalidOperationException("Missing string terminator.");

            return Encoding.UTF8.GetString(result.ToArray());
        }

        public static int GetInt32(ref int _pointer, ReadOnlySpan<byte> instructions)
        {
            var binary = new byte[sizeof(int)];
            binary[0] = instructions[_pointer];

            for (var i = 1; i < sizeof(int); i++)
            {
                _pointer++;
                binary[i] = instructions[_pointer];
            }

            if (BitConverter.IsLittleEndian)
                Array.Reverse(binary);

            var value = BitConverter.ToInt32(binary);
            return value;
        }

        public static InstructionCode GetInstructionCode(byte value)
        {
            var code = (InstructionCode)value;

            if (!Enum.IsDefined(typeof(InstructionCode), code))
            {
                throw new InvalidOperationException("Not an instruction.");
            }

            return code;
        }
    }
}
