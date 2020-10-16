using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vmr.Common.Primitives;

namespace Vmr.Common.Assemble
{
    public sealed class LabelTable
    {
        private readonly IReadOnlyDictionary<IlAddress, string> _references;
        private readonly IReadOnlyDictionary<string, IlAddress> _targets;

        internal LabelTable(IReadOnlyDictionary<IlAddress, string> references, IReadOnlyDictionary<string, IlAddress> targets)
        {
            _references = references;
            _targets = targets;
        }

        public IlAddress GetTarget(string label)
            => _targets[label];

        public bool TryGetReference(IlAddress reference, out string label)
            => _references.TryGetValue(reference, out label);

        public IReadOnlyCollection<IlAddress> GetTargets() 
            => _targets.Values.ToHashSet();

        public IReadOnlyDictionary<IlAddress, string> GetLabelNames() 
            => _targets.ToDictionary(m => m.Value, m => m.Key);

    }
}
