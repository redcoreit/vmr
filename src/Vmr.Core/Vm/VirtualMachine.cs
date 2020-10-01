using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Vmr.Core.Abstractions;
using Vmr.Core.Exceptions;

namespace Vmr.Core
{
    public sealed class VirtualMachine
    {
        private readonly Stack<object> _stack;
        private readonly IReadOnlyList<object> _instructions;

        public VirtualMachine(IReadOnlyList<object> instructions)
        {
            _stack = new Stack<object>();
            _instructions = instructions.ToList();
        }

        public void Execute(IEnumerable<object> data)
        {
            using var enumerator = _instructions.GetEnumerator();
            int instructionIndex = 0;

            while (enumerator.MoveNext())
            {
                if (!InstructionFacts.TryGetInstructionCode(enumerator.Current, out var instruction))
                {
                    throw new VmExecutionException($"Unexpected value found: '{enumerator.Current}'.");
                }

                DispatchInstruction(instructionIndex, instruction.Value, enumerator);
                instructionIndex++;
            }

        }

        public Stack<object> GetStack() => new Stack<object>(_stack);

        private void DispatchInstruction(int instructionIndex, InstructionCode instruction, IEnumerator<object> enumerator)
        {
            switch (instruction)
            {
                case InstructionCode.Add:
                    {
                        Add(instructionIndex, instruction, enumerator);
                        break;
                    }
                case InstructionCode.Ldc:
                    {
                        Ldc(instructionIndex, instruction, enumerator);
                        break;
                    }
                case InstructionCode.Pop:
                    {
                        Pop(instructionIndex, instruction, enumerator);
                        break;
                    }
                default:
                    break;
            }
        }

        private void Add(int instructionIndex, InstructionCode instruction, IEnumerator<object> enumerator)
        {
            if (_stack.Count == 0)
            {
                Throw.StackUnderflowException(instructionIndex);
            }

            var op1 = _stack.Pop();

            if (_stack.Count == 0)
            {
                Throw.StackUnderflowException(instructionIndex);
            }

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

        private void Ldc(int instructionIndex, InstructionCode instruction, IEnumerator<object> enumerator)
        {
            if (!enumerator.MoveNext())
            {
                Throw.MissingInstructionArgument(instructionIndex);
            }

            _stack.Push(enumerator.Current);
        }

        private void Pop(int instructionIndex, InstructionCode instruction, IEnumerator<object> enumerator)
        {
            _stack.Pop();
        }
    }
}
