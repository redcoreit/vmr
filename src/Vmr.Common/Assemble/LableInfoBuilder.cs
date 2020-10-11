using System;
using System.Collections.Generic;
using System.Text;
using Vmr.Common.Instructions;

namespace Vmr.Common.Assemble
{
    internal sealed class LableInfoBuilder
    {
        private readonly Dictionary<int, string> _callSites;
        private readonly Dictionary<string, int> _targets;

        public LableInfoBuilder()
        {
            _callSites = new Dictionary<int, string>();
            _targets = new Dictionary<string, int>();
        }

        public void AddCallSite(int placeholderArgIndex, string label)
        {
            if (_callSites.ContainsKey(placeholderArgIndex))
                throw new InvalidOperationException("Label call site already exsists.");

            _callSites[placeholderArgIndex] = label;
        }

        public void AddTarget(string label, IlRef argument)
        {
            if (_targets.ContainsKey(label))
                throw new InvalidOperationException($"Label '{label}' target already exsists.");

            _targets[label] = argument.Value;
        }

        public LableInfo Build()
            => new LableInfo(_callSites, _targets);
    }
}
