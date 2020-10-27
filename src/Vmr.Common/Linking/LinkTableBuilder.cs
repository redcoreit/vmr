using System;
using System.Collections.Generic;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Common.Linking
{
    internal sealed class LinkTableBuilder
    {
        private readonly Dictionary<IlAddress, string> _references;
        private readonly Dictionary<string, IlAddress> _targets;

        public LinkTableBuilder()
        {
            _references = new Dictionary<IlAddress, string>();
            _targets = new Dictionary<string, IlAddress>();
        }

        public IReadOnlyDictionary<IlAddress, string> References => _references;

        public IReadOnlyDictionary<string, IlAddress> Targets => _targets;

        public void AddReference(IlAddress reference, string name)
        {
            if (_references.ContainsKey(reference))
                throw new InvalidOperationException("Call site already exsists.");

            _references[reference] = name;
        }

        public void AddTarget(string name, IlAddress target)
        {
            if (_targets.ContainsKey(name))
                throw new InvalidOperationException($"Target '{name}' already exsists.");

            _targets[name] = target;
        }

        public LinkTable Build()
            => new LinkTable(References, Targets);
    }
}
