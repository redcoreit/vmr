﻿#pragma warning disable CA1707
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
        private readonly LableInfoBuilder _lableInfoBuilder;

        private int _ilRef;

        public CodeBuilder()
        {
            _code = new List<object>();
            _lableInfoBuilder = new LableInfoBuilder();
        }

        public IlRef IlRef => _ilRef;

        public byte[] Compile()
        {
            var labelInfo = _lableInfoBuilder.Build();
            Linker.LinkLabels(_code, labelInfo);

            var assmebler = new Assembler();
            var binaryCode = assmebler.Emit(GetInstructions());

            return binaryCode;
        }

        public List<object> GetInstructions()
            => _code.ToList();

        public LableInfo GetLabelInfo()
            => _lableInfoBuilder.Build();

        public void Ldc_i4(int value)
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

        public void Br(IlRef target)
        {
            _code.Add(InstructionCode.Br);
            _code.Add(target.Value);

            _ilRef += SizeOfOpCode + sizeof(int);
        }

        public void Br(string label)
        {
            _code.Add(InstructionCode.Br);
            _code.Add(0); // placeholder

            _lableInfoBuilder.AddCallSite(_code.Count - 1, label);
            _ilRef += SizeOfOpCode + sizeof(int);
        }

        public void Ceq()
        {
            _code.Add(InstructionCode.Ceq);
            _ilRef += SizeOfOpCode;
        }

        public void Label(string label)
        {
            _lableInfoBuilder.AddTarget(label, _ilRef);
        }

        public void Nop()
        {
            _code.Add(InstructionCode.Nop);
            _ilRef += SizeOfOpCode;
        }
    }
}
