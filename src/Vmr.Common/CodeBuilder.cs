#pragma warning disable CA1707

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common.Assemble;
using Vmr.Common.Exeptions;
using Vmr.Common.Instructions;
using Vmr.Common.Linking;
using Vmr.Common.ObjectModel;
using Vmr.Common.Primitives;

namespace Vmr.Common
{
    public sealed class CodeBuilder
    {
        private readonly HashSet<string> _methodNames;
        private readonly HashSet<string> _labelNames;
        private readonly Stack<Method> _methods;
        private readonly List<ProgramNode> _nodes;

        private Method? _entryPoint;
        private int _methodOrder = 0;

        public CodeBuilder()
        {
            _methodNames = new HashSet<string>();
            _labelNames = new HashSet<string>();
            _methods = new Stack<Method>();
            _nodes = new List<ProgramNode>();
        }

        public byte[] GetBinaryProgram()
        {
            var ilProgram = GetIlProgram();

            var binaryCode = Assembler.Emit(ilProgram);
            return binaryCode;
        }

        public IlProgram GetIlProgram()
        {
            EndMethod();

            if (_entryPoint is null)
            {
                throw new VmrException("Entry point not found by code builder.");
            }

            var linkedProgram = Linker.Run(_entryPoint, _methods.Reverse().ToArray());
            return linkedProgram;
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

        public void Br(string label)
        {
            Add(InstructionCode.Br);
            AddLabelReference(label);
        }

        public void Ceq()
        {
            Add(InstructionCode.Ceq);
        }

        public void Label(string name)
        {
            if (!_labelNames.Add(name))
            {
                throw new VmrException($"Label '{name}' already exists in current method '{_methods.Peek().Name}' scope.");
            }

            _nodes.Add(new LabelDeclaration(name));
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
            CheckLocalsRange(index);

            Add(InstructionCode.Ldloc);
            Add(index);
        }

        public void Stloc(int index)
        {
            CheckLocalsRange(index);

            Add(InstructionCode.Stloc);
            Add(index);
        }

        public void Method(string name, int locals = 0, bool isEntryPoint = false)
        {
            if (!_methodNames.Add(name))
            {
                throw new VmrException($"Method '{name}' already exists.");
            }

            if (_methods.Count > 0)
            {
                EndMethod();
            }

            var method = new Method(_methodOrder++, Array.Empty<ProgramNode>(), name, locals, isEntryPoint);
            _methods.Push(method);
        }

        public void Ret()
        {
            Add(InstructionCode.Ret);
        }

        public void Call(string name)
        {
            Add(InstructionCode.Call);
            Add(name, sizeof(int));
        }

        public void Comment(string text)
        {
            _nodes.Add(new Comment(text));
        }

        private void Add(InstructionCode instruction)
        {
            _nodes.Add(new Instruction(instruction));
        }

        private void Add(int value)
        {
            _nodes.Add(new Argument(sizeof(int), value));
        }

        private void Add(object obj, int sizeOfObj)
        {
            _nodes.Add(new Argument(sizeOfObj, obj));
        }

        private void AddLabelReference(string name)
        {
            Add(name, sizeof(int));
        }

        private void EndMethod()
        {
            if (_methods.Count == 0)
            {
                throw new VmrException($"Program must contain at least an entry point method.");
            }

            if (_nodes.Count == 0)
            {
                return;
            }

            if (_nodes.All(m => m is Comment))
            {
                return;
            }

            CheckUndefinedLabels();

            var current = _methods.Pop();
            var method = new Method(current.Order, _nodes.ToArray(), current.Name, current.Locals, current.IsEntryPoint);

            if (current.IsEntryPoint)
            {
                if (_entryPoint is object)
                {
                    throw new VmrException($"Multiple entry point not supported.");
                }

                _entryPoint = method;
            }

            _methods.Push(method);
            _nodes.Clear();
            _labelNames.Clear();
        }

        private void CheckLocalsRange(int index)
        {
            if ((uint)index >= (uint)_methods.Peek().Locals)
            {
                throw new VmrException($"Local variable index '{index}' is out of range.");
            }
        }

        private void CheckUndefinedLabels()
        {
            for (int idx = 0; idx < _nodes.Count; idx++)
            {
                if (_nodes[idx] is Instruction inst && InstructionFacts.IsBranchingInstruction(inst.InstructionCode))
                {
                    var name = (string)((Argument)_nodes[++idx]).Value;

                    if (!_labelNames.Contains(name))
                    {
                        throw new VmrException($"Label '{name}' not exists in current method '{_methods.Peek().Name}' scope.");
                    }
                }
            }
        }
    }
}
