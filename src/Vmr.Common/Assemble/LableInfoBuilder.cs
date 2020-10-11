using System;
using System.Collections.Generic;
using System.Text;
using Vmr.Common.Instructions;

namespace Vmr.Common.Assemble
{
    internal sealed class LableInfoBuilder
    {
        private readonly Dictionary<int, string> _labelReferences;
        private readonly Dictionary<string, int> _targets;

        public LableInfoBuilder()
        {
            _labelReferences = new Dictionary<int, string>();
            _targets = new Dictionary<string, int>();
        }

        public void AddReferenceIlRef(IlRef labelReference, string label)
        {
            if (_labelReferences.ContainsKey(labelReference.Value))
                throw new InvalidOperationException("Label call site already exsists.");

            _labelReferences[labelReference.Value] = label;
        }

        public void AddTarget(string label, IlRef target)
        {
            if (_targets.ContainsKey(label))
                throw new InvalidOperationException($"Label '{label}' target already exsists.");

            _targets[label] = target.Value;
        }

        public LableInfo Build()
            => new LableInfo(_labelReferences, _targets);
    }
}
