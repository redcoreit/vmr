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
        private readonly List<object> _code;

        public CodeBuilder()
        {
            _code = new List<object>();
        }

        public IReadOnlyList<byte> Assemble()
        {
            var emitter = new Assembler();
            var binaryCode = emitter.Emit(GetInstructions());

            return binaryCode;
        }

        public IReadOnlyList<object> GetInstructions()
            => _code.ToArray();

        public void Ldc(int value)
        {
            _code.Add(InstructionCode.Ldc);
            _code.Add(value);
        }

        public void Ldc(string value)
        {
            _code.Add(InstructionCode.Ldc);
            _code.Add(value);
        }

        public void Add()
            => _code.Add(InstructionCode.Add);

        public void Pop()
            => _code.Add(InstructionCode.Pop);
    }
}
