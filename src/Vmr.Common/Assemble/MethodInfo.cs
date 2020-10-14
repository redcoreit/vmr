using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vmr.Common.Instructions;

namespace Vmr.Common.Assemble
{
    public sealed class MethodInfo
    {
        private readonly IReadOnlyDictionary<int, string> _references;
        private readonly IReadOnlyDictionary<string, int> _targets;
        private readonly int _entrypoint;
        private readonly IReadOnlyDictionary<int, int> _locals;

        internal MethodInfo(IReadOnlyDictionary<int, string> references, IReadOnlyDictionary<string, int> targets, int entrypoint, IReadOnlyDictionary<int, int> _locals)
        {
            _references = references;
            _targets = targets;
            _entrypoint = entrypoint;
            this._locals = _locals;
        }

        public IlRef Entrypoint => _entrypoint;

        public IlRef GetTarget(string label)
            => _targets[label];

        public bool TryGetReference(int labelReferenceIlRef, out string label)
            => _references.TryGetValue(labelReferenceIlRef, out label);

        public IReadOnlyCollection<int> GetTargets()
            => _targets.Values.ToHashSet();

        public IReadOnlyDictionary<int, string> GetNames()
            => _targets.ToDictionary(m => m.Value, m => m.Key);

    }
}
