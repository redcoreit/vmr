#pragma warning disable CA1707
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common.Assemble;
using Vmr.Common.Exeptions;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Common
{
    public sealed class CodeBuilder
    {
        private readonly List<IlObject> _program;
        private readonly LabelTableBuilder _lableTableBuilder;
        private readonly MethodTableBuilder _methodTableBuilder;

        private int _segment;
        private int _ilRef;

        public CodeBuilder()
        {
            _program = new List<IlObject>();
            _lableTableBuilder = new LabelTableBuilder();
            _methodTableBuilder = new MethodTableBuilder();
        }

        public byte[] GetBinaryProgram()
        {
            var ilProgram = GetIlProgram();

            var binaryCode = Assembler.Emit(ilProgram);
            return binaryCode;
        }

        public IlProgram GetIlProgram()
        {
            var labelTable = _lableTableBuilder.Build();
            var methodTable = _methodTableBuilder.Build();
            Linker.Run(_program, methodTable, labelTable);

            return new IlProgram(_program, labelTable.GetTargets(), labelTable.GetLabelNames());
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
            AddLabelReference(label);
        }

        public void Ceq()
        {
            Add(InstructionCode.Ceq);
        }

        public void Label(string label)
        {
            var address = GetIlAddress();
            _lableTableBuilder.AddTarget(label, address);
        }

        public void Nop()
        {
            Add(InstructionCode.Nop);
        }

        public void Brtrue(string label)
        {
            Add(InstructionCode.Brtrue);
            AddLabelReference(label);
        }

        public void Brfalse(string label)
        {
            Add(InstructionCode.Brfalse);
            AddLabelReference(label);
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

        public void Method(string name, int locals = 0, bool isEntryPoint = false)
        {
            if (_ilRef == 0 && _segment != 0)
            {
                throw new VmrException($"Empty method body detected. Name: '{name}'");
            }

            _segment += _ilRef;
            _ilRef = 0;

            var address = GetIlAddress();
            _methodTableBuilder.AddTarget(name, address, locals, isEntryPoint);
        }

        public void Ret()
        {
            Add(InstructionCode.Ret);
        }

        public void Call(string name)
        {
            Add(InstructionCode.Call);
            AddMethodReference(name);
        }

        private void Add(InstructionCode instruction)
        {
            var address = GetIlAddress();

            _program.Add(new IlObject(address, instruction));
            _ilRef += InstructionFacts.SizeOfOpCode;
        }

        private void Add(int value)
        {
            var address = GetIlAddress();

            _program.Add(new IlObject(address, value));
            _ilRef += sizeof(int);
        }

        private void Add(object obj, int sizeOfObj)
        {
            var address = GetIlAddress();

            _program.Add(new IlObject(address, obj));
            _ilRef += sizeOfObj;
        }

        private void AddLabelReference(string name)
        {
            var address = GetIlAddress();
            _lableTableBuilder.AddReference(address, name);

            _program.Add(new IlObject(address, 0));
            _ilRef += sizeof(int);
        }

        private void AddMethodReference(string name)
        {
            var address = GetIlAddress();
            _methodTableBuilder.AddReference(address, name);

            _program.Add(new IlObject(address, 0));
            _ilRef += sizeof(int);
        }

        private IlAddress GetIlAddress()
            => new IlAddress(_segment, _ilRef);
    }
}
