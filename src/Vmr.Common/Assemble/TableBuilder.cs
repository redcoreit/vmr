using System;
using System.Collections.Generic;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Common.Assemble
{
    internal abstract class TableBuilder<TResult>
        where TResult : notnull
    {
        private readonly Dictionary<IlAddress, string> _references;
        private readonly Dictionary<string, IlAddress> _targets;

        public TableBuilder()
        {
            _references = new Dictionary<IlAddress, string>();
            _targets = new Dictionary<string, IlAddress>();
        }

        protected IReadOnlyDictionary<IlAddress, string> References => _references;

        protected IReadOnlyDictionary<string, IlAddress> Targets => _targets;

        protected virtual void AddReference(IlAddress reference, string name)
        {
            if (_references.ContainsKey(reference))
                throw new InvalidOperationException("Call site already exsists.");

            _references[reference] = name;
        }

        protected virtual void AddTarget(string name, IlAddress target)
        {
            if (_targets.ContainsKey(name))
                throw new InvalidOperationException($"Target '{name}' already exsists.");

            _targets[name] = target;
        }

        public abstract TResult Build();
    }
}
