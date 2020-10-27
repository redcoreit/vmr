using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vmr.Common.Primitives;

namespace Vmr.Common.Linking
{
    public sealed class LinkTable
    {
        private readonly IReadOnlyDictionary<IlAddress, string> _references;
        private readonly IReadOnlyDictionary<string, IlAddress> _targets;

        internal LinkTable(IReadOnlyDictionary<IlAddress, string> references, IReadOnlyDictionary<string, IlAddress> targets)
        {
            _references = references;
            _targets = targets;
        }

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
