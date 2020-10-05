using System;
using System.Collections.Generic;
using System.Text;

namespace Vmr.Instructions
{
    public sealed class LableInfo
    {
        private readonly Dictionary<int, string> _callSites;
        private readonly Dictionary<string, int> _targets;

        public LableInfo(Dictionary<int, string> callSites, Dictionary<string, int> targets)
        {
            _callSites = callSites;
            _targets = targets;
        }

        public IlRef GetTarget(string label)
            => _targets[label];

        public bool TryGetCallSite(int placeholderArgIndex, out string label)
            => _callSites.TryGetValue(placeholderArgIndex, out label);
    }
}
