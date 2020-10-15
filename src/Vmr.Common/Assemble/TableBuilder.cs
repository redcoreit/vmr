using System;
using System.Collections.Generic;
using Vmr.Common.Instructions;

namespace Vmr.Common.Assemble
{
    internal abstract class TableBuilder<TResult, TAddress>
        where TResult : notnull
        where TAddress : struct
    {
        private readonly Dictionary<TAddress, string> _references;
        private readonly Dictionary<string, TAddress> _targets;

        public TableBuilder()
        {
            _references = new Dictionary<TAddress, string>();
            _targets = new Dictionary<string, TAddress>();
        }

        protected IReadOnlyDictionary<TAddress, string> References => _references;

        protected IReadOnlyDictionary<string, TAddress> Targets => _targets;

        protected virtual void AddReference(TAddress reference, string name)
        {
            if (_references.ContainsKey(reference))
                throw new InvalidOperationException("Call site already exsists.");

            _references[reference] = name;
        }

        protected virtual void AddTarget(string name, TAddress target)
        {
            if (_targets.ContainsKey(name))
                throw new InvalidOperationException($"Target '{name}' already exsists.");

            _targets[name] = target;
        }

        public abstract TResult Build();
    }
}
