using System;
using System.Collections.Generic;
using System.Text;

namespace Vmr.Instructions
{
    public static class Linker
    {
        public static void LinkLabels(List<object> code, IReadOnlyDictionary<int, string> labelCallSites, IReadOnlyDictionary<string, int> labelTargets)
        {
            for (int i = 0; i < code.Count; i++)
            {
                if(labelCallSites.TryGetValue(i, out var label))
                {
                    code[i] = labelTargets[label];
                }
            }
        }
    }
}
