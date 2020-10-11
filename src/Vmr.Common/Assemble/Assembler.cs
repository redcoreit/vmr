using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Common.Assemble
{
    public static class Assembler
    {
        public static byte[] Emit(IlProgram program)
        {
            var binaryCode = new List<byte>();

            foreach (var ilObj in program.IlObjects)
            {
                var code = EmitObject(ilObj.Obj);
                binaryCode.AddRange(code);
            }

            return binaryCode.ToArray();
        }

        private static byte[] EmitObject(object obj)
        {
            // TODO (RH -): add decimal type handling
            var code = obj switch
            {
                InstructionCode instruction => BinaryConvert.GetBytes(instruction),
                string text => BinaryConvert.GetBytes(text),
                int value => BinaryConvert.GetBytes(value),
                _ => throw new ArgumentOutOfRangeException(nameof(obj), obj.GetType().Name, null)
            };

            return code;
        }
    }
}
