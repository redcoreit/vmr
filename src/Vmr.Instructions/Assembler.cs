using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vmr.Instructions
{
    internal class Assembler
    {
        public IReadOnlyList<byte> Emit(IReadOnlyList<object> instructions)
        {
            var binaryCode = new List<byte>();

            foreach (var obj in instructions)
            {
                var code = EmitObject(obj);
                binaryCode.AddRange(code);
            }

            return binaryCode;
        }

        private IEnumerable<byte> EmitObject(object obj)
        {
            var code = obj switch
            {
                InstructionCode instruction => EmitInstruction(instruction),
                string text => EmitString(text),
                int value => EmitInt32(value),
                _ => throw new ArgumentOutOfRangeException(nameof(obj), obj.GetType().Name, null)
            };

            return code;
        }

        private IEnumerable<byte> EmitInstruction(InstructionCode instruction)
            => new[] { (byte)instruction };

        private IEnumerable<byte> EmitString(string text)
        {
            var result = Encoding.UTF8.GetBytes(text).ToList();
            result.Insert(0, InstructionFacts.StringInitializer);
            result.Add(InstructionFacts.StringTerminator);

            return result;
        }

        private IEnumerable<byte> EmitInt32(int value)
        {
            byte[] result = BitConverter.GetBytes(value);
            
            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }
    }
}
