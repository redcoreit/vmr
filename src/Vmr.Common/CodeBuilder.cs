﻿#pragma warning disable CA1707
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common.Assemble;
using Vmr.Common.Instructions;

namespace Vmr.Common
{
    public sealed class CodeBuilder
    {
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

            var binaryCode = Assembler.Emit(GetInstructions());

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

            _ilRef += InstructionFacts.SizeOfOpCode + sizeof(int);
        }

        public void Ldstr(string value)
        {
            _code.Add(InstructionCode.Ldstr);
            _code.Add(value);

            // TODO (RH perf): find an efficient way to determine UTF8 string size.
            var sizeofValue = BinaryConvert.GetBytes(value).Length;

            _ilRef += InstructionFacts.SizeOfOpCode + sizeofValue;
        }

        public void Add()
        {
            _code.Add(InstructionCode.Add);
            _ilRef += InstructionFacts.SizeOfOpCode;
        }

        public void Pop()
        {
            _code.Add(InstructionCode.Pop);
            _ilRef += InstructionFacts.SizeOfOpCode;
        }

        public void Br(IlRef target)
        {
            _code.Add(InstructionCode.Br);
            _code.Add(target.Value);

            _ilRef += InstructionFacts.SizeOfOpCode + sizeof(int);
        }

        public void Br(string label)
        {
            _code.Add(InstructionCode.Br);
            _code.Add(0); // placeholder

            _lableInfoBuilder.AddCallSite(_code.Count - 1, label);
            _ilRef += InstructionFacts.SizeOfOpCode + sizeof(int);
        }

        public void Ceq()
        {
            _code.Add(InstructionCode.Ceq);
            _ilRef += InstructionFacts.SizeOfOpCode;
        }

        public void Label(string label)
        {
            _lableInfoBuilder.AddTarget(label, _ilRef);
        }

        public void Nop()
        {
            _code.Add(InstructionCode.Nop);
            _ilRef += InstructionFacts.SizeOfOpCode;
        }

        public void Brtrue(string label)
        {
            _code.Add(InstructionCode.Brtrue);
            _code.Add(0); // placeholder

            _lableInfoBuilder.AddCallSite(_code.Count - 1, label);
            _ilRef += InstructionFacts.SizeOfOpCode + sizeof(int);
        }

        public void Brfalse(string label)
        {
            _code.Add(InstructionCode.Brfalse);
            _code.Add(0); // placeholder

            _lableInfoBuilder.AddCallSite(_code.Count - 1, label);
            _ilRef += InstructionFacts.SizeOfOpCode + sizeof(int);
        }

        public void Ldloc(int index)
        {
            _code.Add(InstructionCode.Ldloc);
            _code.Add(index);

            _ilRef += InstructionFacts.SizeOfOpCode + sizeof(int);
        }

        public void Stloc(int index)
        {
            _code.Add(InstructionCode.Stloc);
            _code.Add(index);

            _ilRef += InstructionFacts.SizeOfOpCode + sizeof(int);
        }
    }
}