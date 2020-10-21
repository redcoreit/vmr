using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Runtime.Exceptions;

namespace Vmr.Runtime.Vm
{
    internal sealed class MethodState<TState>
    {
        private readonly Stack<TState> _stack;

        public MethodState()
        {
            _stack = new Stack<TState>();
        }

        public void Save(TState state)
        {
            _stack.Push(state);
        }

        public TState Restore()
        {
            if (_stack.Count == 0)
            {
                throw new VmExecutionException("No remaining method state found.");
            }

            return _stack.Pop();
        }

        public Stack<TState> GetStates()
            => new Stack<TState>(_stack);

        public void Clear()
        {
            _stack.Clear();
        }
    }
}
