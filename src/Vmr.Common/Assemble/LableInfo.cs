using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vmr.Common.Instructions;

namespace Vmr.Common.Assemble
{
    public sealed class LableInfo
    {
        private readonly Dictionary<int, string> _labelReferences;
        private readonly Dictionary<string, int> _targets;

        internal LableInfo(Dictionary<int, string> labelReferences, Dictionary<string, int> targets)
        {
            _labelReferences = labelReferences;
            _targets = targets;
        }

        public IReadOnlyCollection<int> TargetIlRefs
            => _targets.Values.ToHashSet();

        public IlRef GetTarget(string label)
            => _targets[label];

        public bool TryGetReference(int labelReferenceIlRef, out string label)
            => _labelReferences.TryGetValue(labelReferenceIlRef, out label);
    }
}
