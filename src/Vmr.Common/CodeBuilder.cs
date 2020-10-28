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
            AddNode(InstructionCode.Ldc_i4);
            AddNode(value);
        }

        public void Ldstr(string value)
        {
            AddNode(InstructionCode.Ldstr);

            // TODO (RH perf): find an efficient way to determine UTF8 string size.
            var sizeofValue = BinaryConvert.GetBytes(value).Length;
            AddNode(value, sizeofValue);
        }

        public void Add()
        {
            AddNode(InstructionCode.Add);
        }

        public void Pop()
        {
            AddNode(InstructionCode.Pop);
        }

        public void Br(string label)
        {
            AddNode(InstructionCode.Br);
            AddLabelReference(label);
        }

        public void Ceq()
        {
            AddNode(InstructionCode.Ceq);
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
            AddNode(InstructionCode.Nop);
        }

        public void Brtrue(string label)
        {
            AddNode(InstructionCode.Brtrue);
            AddLabelReference(label);
        }

        public void Brfalse(string label)
        {
            AddNode(InstructionCode.Brfalse);
            AddLabelReference(label);
        }

        public void Ldloc(int index)
        {
            CheckLocalsRange(index);

            AddNode(InstructionCode.Ldloc);
            AddNode(index);
        }

        public void Ldarg(int index)
        {
            CheckArgsRange(index);

            AddNode(InstructionCode.Ldarg);
            AddNode(index);
        }

        public void Stloc(int index)
        {
            CheckLocalsRange(index);

            AddNode(InstructionCode.Stloc);
            AddNode(index);
        }

        public void Method(string name, int locals = 0, int args = 0, bool isEntryPoint = false)
        {
            if (!_methodNames.Add(name))
            {
                throw new VmrException($"Method '{name}' already exists.");
            }

            if (_methods.Count > 0)
            {
                EndMethod();
            }

            var method = new Method(_methodOrder++, Array.Empty<ProgramNode>(), name, locals, args, isEntryPoint);
            _methods.Push(method);
        }

        public void Ret()
        {
            AddNode(InstructionCode.Ret);
        }

        public void Call(string name)
        {
            AddNode(InstructionCode.Call);
            AddNode(name, sizeof(int));
        }

        public void Comment(string text)
        {
            _nodes.Add(new Comment(text));
        }

        private void AddNode(InstructionCode instruction)
        {
            _nodes.Add(new Instruction(instruction));
        }

        private void AddNode(int value)
        {
            _nodes.Add(new Argument(sizeof(int), value));
        }

        private void AddNode(object obj, int sizeOfObj)
        {
            _nodes.Add(new Argument(sizeOfObj, obj));
        }

        private void AddLabelReference(string name)
        {
            AddNode(name, sizeof(int));
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
            var method = new Method(current.Order, _nodes.ToArray(), current.Name, current.Locals, current.Args, current.IsEntryPoint);

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

        private void CheckArgsRange(int index)
        {
            if ((uint)index >= (uint)_methods.Peek().Args)
            {
                throw new VmrException($"Method argument index '{index}' is out of range.");
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
