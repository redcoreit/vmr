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

        internal VirtualMachine(IEnumerable<object> instructions)
        {
            _stack = new Stack<object>();
            _instructions = instructions.ToList();
        }

        internal void Execute(IEnumerable<object> data)
        {
            using var enumerator = _instructions.GetEnumerator();

            while(enumerator.MoveNext())
            {
                if(!InstructionFacts.TryGetInstructionCode(enumerator.Current, out var instruction))
                {
                    throw new VmExecutionException($"Unexpected value found: '{enumerator.Current}'.");
                }

                DispatchInstruction(instruction.Value, enumerator);
            }
            
        }

        public Stack<object> GetStack() => new Stack<object>(_stack);

        private void DispatchInstruction(InstructionCode instruction, IEnumerator<object> enumerator)
        {
            switch (instruction)
            {
                case InstructionCode.Add:
                    {
                        Add(instruction, enumerator);
                        break;
                    }
                case InstructionCode.Ldc:
                    {
                        Ldc(instruction, enumerator);
                        break;
                    }
                case InstructionCode.Pop:
                    {
                        Pop(instruction, enumerator);
                        break;
                    }
                default:
                    break;
            }
        }

        private void Add(InstructionCode instruction, IEnumerator<object> enumerator)
        {
            if (_stack.Count == 0)
            {
                Throw.StackUnderflowException(instruction);
            }

            var op1 = _stack.Pop();

            if (_stack.Count == 0)
            {
                Throw.StackUnderflowException(instruction);
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

        private void Ldc(InstructionCode instruction, IEnumerator<object> enumerator)
        {
            if(!enumerator.MoveNext())
            {
                Throw.MissingInstructionArgument(instruction);
            }

            _stack.Push(enumerator.Current);
        }

        private void Pop(InstructionCode instruction, IEnumerator<object> enumerator)
        {
            _stack.Pop();
        }
    }
}
