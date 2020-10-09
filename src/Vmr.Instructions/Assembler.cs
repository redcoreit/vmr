﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vmr.Instructions
{
    public static class Assembler
    {
        public static byte[] Emit(IReadOnlyList<object> instructions)
        {
            var binaryCode = new List<byte>();

            foreach (var obj in instructions)
            {
                var code = EmitObject(obj);
                binaryCode.AddRange(code);
            }

            return binaryCode.ToArray();
        }

        private static byte[] EmitObject(object obj)
        {
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
