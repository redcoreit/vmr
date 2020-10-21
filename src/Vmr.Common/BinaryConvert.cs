using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Vmr.Common.Instructions;

namespace Vmr.Common
{
    public static class BinaryConvert
    {
        public static byte[] GetBytes(InstructionCode instruction)
            => new[] { (byte)instruction };

        public static byte[] GetBytes(string text)
        {
            var result = Encoding.UTF8.GetBytes(text).ToList();
            result.Add(InstructionFacts.Eos);

            return result.ToArray();
        }

        public static byte[] GetBytes(int value)
        {
            var result = BitConverter.GetBytes(value);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        public static string GetString(ref int pointer, ReadOnlySpan<byte> instructions)
        {
            if (instructions[pointer] == InstructionFacts.Eos)
                return string.Empty;

            var result = new List<byte>();

            while (pointer < instructions.Length)
            {
                if (instructions[pointer] == InstructionFacts.Eos)
                {
                    pointer++;
                    break;
                }
                
                result.Add(instructions[pointer]);
                pointer++;
            }

            return Encoding.UTF8.GetString(result.ToArray());
        }

        public static int GetInt32(ref int pointer, ReadOnlySpan<byte> instructions)
        {
            var binary = new byte[sizeof(int)];

            for (var i = 0; i < sizeof(int); i++)
            {
                binary[i] = instructions[pointer];
                pointer++;
            }

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(binary);

            var value = BitConverter.ToInt32(binary);
            return value;
        }

        public static InstructionCode GetInstructionCode(ref int pointer, ReadOnlySpan<byte> instructions)
        {
            var value = instructions[pointer];
            var code = (InstructionCode)value;

            if (!Enum.IsDefined(typeof(InstructionCode), code))
                throw new InvalidOperationException("Not an instruction.");

            pointer++;
            return code;
        }

        public static InstructionCode GetInstructionCode(byte value)
        {
            var code = (InstructionCode)value;

            if (!Enum.IsDefined(typeof(InstructionCode), code))
                throw new InvalidOperationException("Not an instruction.");

            return code;
        }
    }
}
