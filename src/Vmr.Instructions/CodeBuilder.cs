using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Instructions
{
    public sealed class CodeBuilder
    {
        private const int SizeOfOpCode = sizeof(InstructionCode);
        private const int SizeOfEos = sizeof(Char);
        private readonly List<object> _code;

        private int _ilRef;

        public CodeBuilder()
        {
            _code = new List<object>();
        }

        public IlRef IlRef => _ilRef;

        public byte[] Assemble()
        {
            var emitter = new Assembler();
            var binaryCode = emitter.Emit(GetInstructions());

            return binaryCode;
        }

        public IReadOnlyList<object> GetInstructions()
            => _code.ToArray();

        public void Ldc(int value)
        {
            _code.Add(InstructionCode.Ldc_i4);
            _code.Add(value);

            _ilRef += SizeOfOpCode + sizeof(int);
        }

        public void Ldstr(string value)
        {
            _code.Add(InstructionCode.Ldstr);
            _code.Add(value);

            // TODO (RH perf): find an efficient way to determine UTF8 string size.
            var sizeofValue = BinaryConvert.GetBytes(value).Length;

            _ilRef += SizeOfOpCode + sizeofValue + SizeOfEos;
        }

        public void Add()
        {
            _code.Add(InstructionCode.Add);
            _ilRef += SizeOfOpCode;
        }

        public void Pop()
        {
            _code.Add(InstructionCode.Pop);
            _ilRef += SizeOfOpCode;
        }
    }
}
