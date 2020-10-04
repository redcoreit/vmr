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

        public static string GetString(IEnumerator<byte> enumerator)
        {
            if(enumerator.Current == InstructionFacts.StringTerminator)
            {
                return string.Empty;
            }

            var result = new List<byte>();

            do
            {
                result.Add(enumerator.Current);
            } while (enumerator.MoveNext() && enumerator.Current != InstructionFacts.StringTerminator);

            if (enumerator.Current != InstructionFacts.StringTerminator)
                throw new InvalidOperationException("Missing string terminator.");

            return Encoding.UTF8.GetString(result.ToArray());
        }

        public static int GetInt32(IEnumerator<byte> enumerator)
        {
            var binary = new byte[sizeof(int)];
            binary[0] = enumerator.Current;

            for (var i = 1; i < sizeof(int); i++)
            {
                enumerator.MoveNext();
                binary[i] = enumerator.Current;
            }

            if (BitConverter.IsLittleEndian)
                Array.Reverse(binary);

            var value = BitConverter.ToInt32(binary);
            return value;
        }

        public static InstructionCode GetInstructionCode(byte value)
        {
            var code = (InstructionCode)value;

            if(!Enum.IsDefined(typeof(InstructionCode), code))
            {
                throw new InvalidOperationException("Not an instruction.");
            }

            return code;
        }
    }
}
