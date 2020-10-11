#pragma warning disable CA1707
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common.Assemble;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Common
{
    public sealed class CodeBuilder
    {
        private readonly List<IlObject> _program;
        private readonly LableInfoBuilder _lableInfoBuilder;

        private int _ilRef;

        public CodeBuilder()
        {
            _program = new List<IlObject>();
            _lableInfoBuilder = new LableInfoBuilder();
        }

        public IlRef IlRef => _ilRef;

        public byte[] GetBinaryProgram()
        {
            var ilProgram = GetIlProgram();

            var binaryCode = Assembler.Emit(ilProgram);
            return binaryCode;
        }

        public IlProgram GetIlProgram()
        {
            var labelInfo = _lableInfoBuilder.Build();
            Linker.LinkLabels(_program, labelInfo);

            return new IlProgram(_program, labelInfo.TargetIlRefs);
        }

        public void Ldc_i4(int value)
        {
            Add(InstructionCode.Ldc_i4);
            Add(value);
        }

        public void Ldstr(string value)
        {
            Add(InstructionCode.Ldstr);

            // TODO (RH perf): find an efficient way to determine UTF8 string size.
            var sizeofValue = BinaryConvert.GetBytes(value).Length;
            Add(value, sizeofValue);
        }

        public void Add()
        {
            Add(InstructionCode.Add);
        }

        public void Pop()
        {
            Add(InstructionCode.Pop);
        }

        public void Br(IlRef target)
        {
            Add(InstructionCode.Br);
            Add(target.Value);
        }

        public void Br(string label)
        {
            Add(InstructionCode.Br);
            Add(0, label); // placeholder
        }

        public void Ceq()
        {
            Add(InstructionCode.Ceq);
        }

        public void Label(string label)
        {
            _lableInfoBuilder.AddTarget(label, _ilRef);
        }

        public void Nop()
        {
            Add(InstructionCode.Nop);
        }

        public void Brtrue(string label)
        {
            Add(InstructionCode.Brtrue);            
            Add(0, label); // placeholder            
        }

        public void Brfalse(string label)
        {
            Add(InstructionCode.Brfalse);
            Add(0, label); // placeholder
        }

        public void Ldloc(int index)
        {
            Add(InstructionCode.Ldloc);
            Add(index);
        }

        public void Stloc(int index)
        {
            Add(InstructionCode.Stloc);
            Add(index);
        }

        private void Add(InstructionCode instruction)
        {
            _program.Add(new IlObject(_ilRef, instruction));
            _ilRef += InstructionFacts.SizeOfOpCode;
        }

        private void Add(int value, string? labelReference = null)
        {
            if(labelReference is object)
            {
                _lableInfoBuilder.AddReferenceIlRef(_ilRef, labelReference);
            }

            _program.Add(new IlObject(_ilRef, value));
            _ilRef += sizeof(int);
        }

        private void Add(object obj, int sizeOfObj)
        {
            _program.Add(new IlObject(_ilRef, obj));
            _ilRef += sizeOfObj;
        }
    }
}
