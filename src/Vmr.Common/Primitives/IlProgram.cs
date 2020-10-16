using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common.Exeptions;

namespace Vmr.Common.Primitives
{
    public sealed class IlProgram
    {
        internal IlProgram(IlAddress entryPoint, IReadOnlyList<IlMethod> ilMethods, IReadOnlyCollection<IlAddress> labelTargets)
            : this(entryPoint, ilMethods, labelTargets, new Dictionary<IlAddress, string>())
        {
        }

        internal IlProgram(IlAddress entryPoint, IReadOnlyList<IlMethod> ilMethods, IReadOnlyCollection<IlAddress> labelTargets, IReadOnlyDictionary<IlAddress, string> labelNames)
        {
            EntryPoint = entryPoint;
            IlMethods = ilMethods;
            LabelTargets = labelTargets;
            LabelNames = labelNames;
        }

        public IlAddress EntryPoint { get; }

        public IReadOnlyList<IlMethod> IlMethods { get; }

        public IReadOnlyCollection<IlAddress> LabelTargets { get; }

        public IReadOnlyDictionary<IlAddress, string> LabelNames { get; }
    }
}
