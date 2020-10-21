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
        private readonly MethodState<Dictionary<int, object>> _methodState;
        private readonly StackFrame<object> _stackFrame;

        private Dictionary<int, object> _locals;
        private int _pointer = 0;

        public VirtualMachine()
        {
            _locals = new();
            _methodState = new();
            _stackFrame = new();
        }

        public void Execute(byte[] program)
        {
            _stackFrame.Clear();
            var span = program.AsSpan();

            var entryPointAddress = BinaryConvert.GetInt32(ref _pointer, program);
            _pointer = entryPointAddress;

            while (_pointer < span.Length)
            {
                var instruction = GetInstruction(span);
                DispatchInstruction(instruction, span);
            }
        }

        public Stack<object> GetStack() => _stackFrame.GetEvaluationStack();

        private InstructionCode GetInstruction(ReadOnlySpan<byte> program)
        {
            try
            {
                return BinaryConvert.GetInstructionCode(ref _pointer, program);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex.Message);
                Throw.NotSupportedInstruction(_pointer - InstructionFacts.SizeOfOpCode, program[_pointer]);
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
                case InstructionCode.Call:
                    {
                        Call(instruction, program);
                        break;
                    }

                case InstructionCode.Ret:
                    {
                        Ret(instruction, program);
                        break;
                    }
                default:
                    {
                        Throw.NotSupportedInstruction(_pointer - InstructionFacts.SizeOfOpCode, (byte)instruction);
                        break;
                    }
            }
        }

        private void Add(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            if (_stackFrame.StackSize == 0)
                Throw.StackUnderflowException(_pointer);

            var op1 = _stackFrame.Pop();

            if (_stackFrame.StackSize == 0)
                Throw.StackUnderflowException(_pointer);

            var op2 = _stackFrame.Pop();

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
            _stackFrame.Push(result);

        }

        private void Ldc_i4(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out int value);
            _stackFrame.Push(value);


        }

        private void Ldstr(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out string value);
            _stackFrame.Push(value);

        }

        private void Pop(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            _stackFrame.Pop();

        }

        private void Br(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out int target);

            if (target >= program.Length)
            {
                Throw.InvalidInstructionArgument(_pointer);
                return;
            }

            _pointer = target;
        }

        private void Ceq(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            var op2 = _stackFrame.Pop();
            var op1 = _stackFrame.Pop();

            var result = Equals(op1, op2);

            _stackFrame.Push(result ? 1 : 0);
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
                Throw.InvalidInstructionArgument(_pointer);
                return;
            }

            var obj = _stackFrame.Pop();
            var isTrue = obj switch
            {
                bool value => value,
                int value => value != 0,
                decimal value => value != 0,
                string value => value is not null,
                null => false,  // Object reference check
                _ => throw new VmExecutionException($"Instructuin not supports object type '{obj.GetType()}'.")
            };

            if (isTrue == expectedCondition)
            {
                _pointer = target;
            }
        }

        private void Ldloc(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out int index);

            if (!_locals.TryGetValue(index, out var value))
            {
                Throw.LocalVariableNotSet(_pointer);
            }

            _stackFrame.Push(value);

        }

        private void Stloc(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out int index);

            var value = _stackFrame.Pop();
            _locals[index] = value;

        }

        private void Call(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            GetArg(program, out int address);

            if ((uint)address >= (uint)program.Length)
            {
                throw new VmExecutionException($"Invalid method address '0x{address.ToString("X4")}'.");
            }

            _methodState.Save(_locals);
            _stackFrame.AddStack();
            _stackFrame.Push(_pointer);
            _pointer = address;
        }

        private void Ret(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            switch (_stackFrame.StackSize)
            {
                case 1:
                    {
                        var retAddress = (int)_stackFrame.Pop();
                        _pointer = retAddress;
                        _stackFrame.DropStack();
                        break;
                    }
                case 2:
                    {
                        var retValue = _stackFrame.Pop();
                        var retAddress = (int)_stackFrame.Pop();
                        _pointer = retAddress;
                        _stackFrame.DropStack();
                        _stackFrame.Push(retValue);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(_stackFrame.StackSize), _stackFrame.StackSize, null);
            }

            _locals = _methodState.Restore();
        }

        private void GetArg(ReadOnlySpan<byte> program, out int value)
        {
            if (_pointer >= program.Length)
                Throw.MissingInstructionArgument(_pointer);

            value = BinaryConvert.GetInt32(ref _pointer, program);
        }

        private void GetArg(ReadOnlySpan<byte> program, out string value)
        {
            if (_pointer >= program.Length)
                Throw.MissingInstructionArgument(_pointer);

            value = BinaryConvert.GetString(ref _pointer, program);
        }
    }
}
