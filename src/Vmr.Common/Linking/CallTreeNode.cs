using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Vmr.Common.Primitives;

namespace Vmr.Common.Linking
{
    internal sealed class CallTreeNode
    {
        public CallTreeNode(Method method)
        {
            Method = method;
            References = Array.Empty<CallTreeNode>();
        }

        public CallTreeNode(Method method, IReadOnlyList<CallTreeNode> references)
        {
            Method = method;
            References = references;
        }

        public Method Method { get; }

        public IReadOnlyList<CallTreeNode> References { get; }
    }
}
