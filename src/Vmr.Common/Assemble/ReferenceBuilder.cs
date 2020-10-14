using System;
using System.Collections.Generic;
using Vmr.Common.Instructions;

namespace Vmr.Common.Assemble
{
    internal abstract class ReferenceBuilder<TResult>
        where TResult : notnull
    {
        private readonly Dictionary<int, string> _references;
        private readonly Dictionary<string, int> _targets;

        public ReferenceBuilder()
        {
            _references = new Dictionary<int, string>();
            _targets = new Dictionary<string, int>();
        }

        protected IReadOnlyDictionary<int, string> References => _references;

        protected IReadOnlyDictionary<string, int> Targets => _targets;

        protected virtual void AddReference(int reference, string name)
        {
            if (_references.ContainsKey(reference))
                throw new InvalidOperationException("Call site already exsists.");

            _references[reference] = name;
        }

        protected virtual void AddTarget(string name, int target)
        {
            if (_targets.ContainsKey(name))
                throw new InvalidOperationException($"Target '{name}' already exsists.");

            _targets[name] = target;
        }

        public abstract TResult Build();
    }
}
