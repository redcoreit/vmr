using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Runtime.Exceptions;

namespace Vmr.Runtime.Vm
{
    internal sealed class StackFrame<TItem>
    {
        private readonly Stack<TItem> _evaluationStack;
        private readonly Stack<int> _basePointerStack;

        public StackFrame()
        {
            _evaluationStack = new Stack<TItem>();
            _basePointerStack = new Stack<int>();
            _basePointerStack.Push(0);
        }

        public int StackSize => _evaluationStack.Count - _basePointerStack.Peek();

        public void Push(TItem item)
        {
            _evaluationStack.Push(item);
        }

        public TItem Pop()
        {
            if(_basePointerStack.Peek() >= _evaluationStack.Count)
            {
                throw new VmExecutionException("StackFrame underflow detected.");
            }

            return _evaluationStack.Pop();
        }

        public void AddStack()
        {
            _basePointerStack.Push(_evaluationStack.Count);
        }

        public void DropStack()
        {
            if(StackSize != 0)
            {
                throw new VmExecutionException("Stack is not empty therefore canno be dropped.");
            }

            _basePointerStack.Pop();
        }

        public Stack<TItem> GetEvaluationStack()
            => new Stack<TItem>(_evaluationStack);

        public void Clear()
        {
            _evaluationStack.Clear();
            _basePointerStack.Clear();
            _basePointerStack.Push(0);
        }
    }
}
