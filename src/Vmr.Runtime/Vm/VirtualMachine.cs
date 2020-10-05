using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Vmr.Instructions;
using Vmr.Runtime.Exceptions;

namespace Vmr.Runtime.Vm
{
    public sealed class VirtualMachine
    {
        private readonly Stack<object> _stack;

        public VirtualMachine()
        {
            _stack = new Stack<object>();
        }

        public void Execute(byte[] instructions)
        {
            _stack.Clear();

            using var enumerator = instructions.AsEnumerable().GetEnumerator();
            var ilIndex = 0;

            while (enumerator.MoveNext())
            {
                var instruction = GetInstruction(ilIndex, enumerator.Current);

                DispatchInstruction(ilIndex, instruction, enumerator);
                ilIndex++;
            }
        }

        public Stack<object> GetStack() => new Stack<object>(_stack);

        private InstructionCode GetInstruction(IlRef ilRef, byte current)
        {
            try
            {
                return BinaryConvert.GetInstructionCode(current);
            }
            catch (InvalidOperationException)
            {
                Throw.NotSupportedInstruction(ilRef);
                throw; // can't happen
            }
        }

        private void DispatchInstruction(IlRef ilRef, InstructionCode instruction, IEnumerator<byte> enumerator)
        {
            switch (instruction)
            {
                case InstructionCode.Add:
                    {
                        Add(ilRef, instruction, enumerator);
                        break;
                    }
                case InstructionCode.Ldc_i4:
                    {
                        Ldc(ilRef, instruction, enumerator);
                        break;
                    }
                case InstructionCode.Ldstr:
                    {
                        Ldstr(ilRef, instruction, enumerator);
                        break;
                    }
                case InstructionCode.Pop:
                    {
                        Pop(ilRef, instruction, enumerator);
                        break;
                    }
                default:
                    break;
            }
        }

        private void Add(IlRef ilRef, InstructionCode instruction, IEnumerator<byte> enumerator)
        {
            if (_stack.Count == 0)
                Throw.StackUnderflowException(ilRef);

            var op1 = _stack.Pop();

            if (_stack.Count == 0)
                Throw.StackUnderflowException(ilRef);

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
        }

        private void Ldc(IlRef ilRef, InstructionCode instruction, IEnumerator<byte> enumerator)
        {
            if (!enumerator.MoveNext())
                Throw.MissingInstructionArgument(ilRef);

            var value = BinaryConvert.GetInt32(enumerator);
            _stack.Push(value);
        }

        private void Ldstr(IlRef ilRef, InstructionCode instruction, IEnumerator<byte> enumerator)
        {
            if (!enumerator.MoveNext())
                Throw.MissingInstructionArgument(ilRef);

            var value = BinaryConvert.GetString(enumerator);
            _stack.Push(value);
        }

        private void Pop(IlRef ilRef, InstructionCode instruction, IEnumerator<byte> enumerator)
        {
            _stack.Pop();
        }
    }
}
