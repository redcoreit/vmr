using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common.Exeptions;
using Vmr.Common.Instructions;

namespace Vmr.Common.Primitives
{
    public sealed class IlProgram
    {
        internal IlProgram(IReadOnlyList<IlObject> ilObjects, IReadOnlyCollection<IlAddress> labelTargets)
            : this(ilObjects, labelTargets, new Dictionary<IlAddress, string>())
        {
        }

        internal IlProgram(IReadOnlyList<IlObject> ilObjects, IReadOnlyCollection<IlAddress> labelTargets, IReadOnlyDictionary<IlAddress, string> labelNames)
        {
            IlObjects = ilObjects;
            LabelTargets = labelTargets;
            LabelNames = labelNames;
        }

        public IReadOnlyList<IlObject> IlObjects { get; }

        public IReadOnlyCollection<IlAddress> LabelTargets { get; }

        public IReadOnlyDictionary<IlAddress, string> LabelNames { get; }
    }
}
