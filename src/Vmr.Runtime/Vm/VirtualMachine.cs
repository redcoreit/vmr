using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Vmr.Common;
using Vmr.Common.Instructions;
using Vmr.Runtime.Exceptions;

namespace Vmr.Runtime.Vm
{
    public sealed class VirtualMachine
    {
        private readonly Dictionary<int, object> _locals;
        private readonly Stack<object> _stack;

        private int _pointer = 0;

        public VirtualMachine()
        {
            _locals = new();
            _stack = new();
        }

        public void Execute(byte[] program)
        {
            _stack.Clear();
            var span = program.AsSpan();

            while (_pointer < span.Length)
            {
                var instruction = GetInstruction(span[_pointer]);
                DispatchInstruction(instruction, span);
            }
        }

        public Stack<object> GetStack() => new Stack<object>(_stack);

        private InstructionCode GetInstruction(byte current)
        {
            try
            {
                return BinaryConvert.GetInstructionCode(current);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex.Message);
                Throw.NotSupportedInstruction((uint)_pointer, current);
                throw; // can't happen
            }
        }

        private void DispatchInstruction(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            switch (instruction)
            {
                case InstructionCode.Add:
                    {
                        Add(instruction, program);
                        break;
                    }
                case InstructionCode.Ldc_i4:
                    {
                        Ldc_i4(instruction, program);
                        break;
                    }
                case InstructionCode.Ldstr:
                    {
                        Ldstr(instruction, program);
                        break;
                    }
                case InstructionCode.Pop:
                    {
                        Pop(instruction, program);
                        break;
                    }
                case InstructionCode.Br:
                    {
                        Br(instruction, program);
                        break;
                    }
                case InstructionCode.Brfalse:
                    {
                        Brfalse(instruction, program);
                        break;
                    }
                case InstructionCode.Brtrue:
                    {
                        Brtrue(instruction, program);
                        break;
                    }
                case InstructionCode.Ceq:
                    {
                        Ceq(instruction, program);
                        break;
                    }
                case InstructionCode.Nop:
                    {
                        _pointer++;
                        break;
                    }
                case InstructionCode.Ldloc:
                    {
                        Ldloc(instruction, program);
                        break;
                    }
                case InstructionCode.Stloc:
                    {
                        Stloc(instruction, program);
                        break;
                    }
                default:
                    {
                        Throw.NotSupportedInstruction((uint)_pointer, (byte)instruction);
                        break;
                    }
            }
        }

        private void Add(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            if (_stack.Count == 0)
                Throw.StackUnderflowException((uint)_pointer);

            var op1 = _stack.Pop();

            if (_stack.Count == 0)
                Throw.StackUnderflowException((uint)_pointer);

            var op2 = _stack.Pop();

            if (op1 is not int num1)
            {
                Throw.OperationNotSupported(instruction, op1, op2);
                return;
            }

            if (op2 is not int num2)
            {
                Throw.OperationNotSupported(instruction, op1, op2);
                return;
            }

            var result = num1 + num2;
            _stack.Push(result);

            _pointer++;
        }

        private void Ldc_i4(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out int value);
            _stack.Push(value);

            _pointer++;
        }

        private void Ldstr(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out string value);
            _stack.Push(value);

            _pointer++;
        }

        private void Pop(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            _stack.Pop();

            _pointer++;
        }

        private void Br(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out int target);

            if (target >= program.Length)
            {
                Throw.InvalidInstructionArgument((uint)_pointer);
                return;
            }

            _pointer = target;
        }

        private void Ceq(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            var op2 = _stack.Pop();
            var op1 = _stack.Pop();

            var result = Equals(op1, op2);

            _stack.Push(result ? 1 : 0);
            _pointer++;
        }

        private void Brtrue(InstructionCode instruction, ReadOnlySpan<byte> program)
            => BrCondition(instruction, program, true);

        private void Brfalse(InstructionCode instruction, ReadOnlySpan<byte> program)
            => BrCondition(instruction, program, false);

        private void BrCondition(InstructionCode instruction, ReadOnlySpan<byte> program, bool expectedCondition)
        {
            GetArg(program, out int target);

            if (target >= program.Length)
            {
                Throw.InvalidInstructionArgument((uint)_pointer);
                return;
            }

            var obj = _stack.Pop();
            var isTrue = obj switch
            {
                bool value => value,
                int value => value != 0,
                decimal value => value != 0,
                string value => value is not null,
                null => false,  // Object reference check
                _ => throw new VmExecutionException($"Instructuin not supports object type '{obj.GetType()}'.")
            };

            _pointer = isTrue == expectedCondition
                ? target
                : (_pointer + 1);
        }

        private void Ldloc(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out int index);

            if (!_locals.TryGetValue(index, out var value))
            {
                Throw.LocalVariableNotSet((uint)_pointer);
            }

            _stack.Push(value);

            _pointer++;
        }

        private void Stloc(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out int index);

            var value = _stack.Pop();
            _locals[index] = value;

            _pointer++;
        }

        private void GetArg(ReadOnlySpan<byte> program, out int value)
        {
            if (_pointer++ >= program.Length)
                Throw.MissingInstructionArgument((uint)_pointer);

            value = BinaryConvert.GetInt32(ref _pointer, program);
        }

        private void GetArg(ReadOnlySpan<byte> program, out string value)
        {
            if (_pointer++ >= program.Length)
                Throw.MissingInstructionArgument((uint)_pointer);

            value = BinaryConvert.GetString(ref _pointer, program);
        }
    }
}
