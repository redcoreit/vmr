using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Vmr.Instructions;
using Vmr.Runtime.Exceptions;

namespace Vmr.Runtime.Vm
{
    public sealed class VirtualMachine
    {
        private readonly Stack<object> _stack;

        private int _pointer = 0;

        public VirtualMachine()
        {
            _stack = new Stack<object>();
        }

        public void Execute(byte[] instructions)
        {
            _stack.Clear();
            var span = instructions.AsSpan();

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
            catch (InvalidOperationException)
            {
                Throw.NotSupportedInstruction(_pointer);
                throw; // can't happen
            }
        }

        private void DispatchInstruction(InstructionCode instruction, ReadOnlySpan<byte> instructions)
        {
            switch (instruction)
            {
                case InstructionCode.Add:
                    {
                        Add(instruction, instructions);
                        break;
                    }
                case InstructionCode.Ldc_i4:
                    {
                        Ldc_i4(instruction, instructions);
                        break;
                    }
                case InstructionCode.Ldstr:
                    {
                        Ldstr(instruction, instructions);
                        break;
                    }
                case InstructionCode.Pop:
                    {
                        Pop(instruction, instructions);
                        break;
                    }
                case InstructionCode.Br:
                    {
                        Br(instruction, instructions);
                        break;
                    }
                case InstructionCode.Ceq:
                    {
                        Ceq(instruction, instructions);
                        break;
                    }
                case InstructionCode.Nop:
                    {
                        _pointer++;
                        break;
                    }
                default:
                    break;
            }
        }

        private void Add(InstructionCode instruction, ReadOnlySpan<byte> instructions)
        {
            if (_stack.Count == 0)
                Throw.StackUnderflowException(_pointer);

            var op1 = _stack.Pop();

            if (_stack.Count == 0)
                Throw.StackUnderflowException(_pointer);

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

        private void Ldc_i4(InstructionCode instruction, ReadOnlySpan<byte> instructions)
        {
            if (_pointer++ >= instructions.Length)
                Throw.MissingInstructionArgument(_pointer);

            var value = BinaryConvert.GetInt32(ref _pointer, instructions);
            _stack.Push(value);

            _pointer++;
        }

        private void Ldstr(InstructionCode instruction, ReadOnlySpan<byte> instructions)
        {
            if (_pointer++ >= instructions.Length)
                Throw.MissingInstructionArgument(_pointer);

            var value = BinaryConvert.GetString(ref _pointer, instructions);
            _stack.Push(value);

            _pointer++;
        }

        private void Pop(InstructionCode instruction, ReadOnlySpan<byte> instructions)
        {
            _stack.Pop();

            _pointer++;
        }

        private void Br(InstructionCode instruction, ReadOnlySpan<byte> instructions)
        {
            if (_pointer++ >= instructions.Length)
                Throw.MissingInstructionArgument(_pointer);

            var target = BinaryConvert.GetInt32(ref _pointer, instructions);

            if (target >= instructions.Length)
            {
                Throw.InvalidInstructionArgument(_pointer);
                return;
            }

            _pointer = target;
        }

        private void Ceq(InstructionCode instruction, ReadOnlySpan<byte> instructions)
        {
            if (_pointer++ >= instructions.Length)
                Throw.MissingInstructionArgument(_pointer);

            var op2 = _stack.Pop();
            var op1 = _stack.Pop();

            var result = Equals(op1, op2);
            
            _stack.Push(result ? 1 : 0);
            _pointer++;
        }
    }
}
