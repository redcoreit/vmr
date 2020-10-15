using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vmr.Common.Instructions;

namespace Vmr.Common.Assemble
{
    public sealed class MethodTable
    {
        private readonly IlAddress _entrypoint;
        private readonly IReadOnlyDictionary<IlAddress, string> _references;
        private readonly IReadOnlyDictionary<string, IlAddress> _targets;
        private readonly IReadOnlyDictionary<IlAddress, int> _locals;

        internal MethodTable(IReadOnlyDictionary<IlAddress, string> references, IReadOnlyDictionary<string, IlAddress> targets, IlAddress entrypoint, IReadOnlyDictionary<IlAddress, int> _locals)
        {
            _references = references;
            _targets = targets;
            _entrypoint = entrypoint;
            this._locals = _locals;
        }

        public IlAddress Entrypoint => _entrypoint;

        public IlAddress GetTarget(string name)
            => _targets[name];

        public bool HasReference(string method)
            => _references.Values.Contains(method);

        public bool TryGetReference(IlAddress reference, out string method)
            => _references.TryGetValue(reference, out method);

        public IReadOnlyCollection<IlAddress> GetTargets()
            => _targets.Values.ToHashSet();

        public IReadOnlyDictionary<IlAddress, string> GetNames()
            => _targets.ToDictionary(m => m.Value, m => m.Key);

    }
}
