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
        internal IlProgram(IReadOnlyList<IlObject> ilObjects, IReadOnlyCollection<int> labelTargetIlRefs)
            : this(ilObjects, labelTargetIlRefs, new Dictionary<int, string>())
        {
        }

        internal IlProgram(IReadOnlyList<IlObject> ilObjects, IReadOnlyCollection<int> labelTargetIlRefs, IReadOnlyDictionary<int, string> labelNames)
        {
            IlObjects = ilObjects;
            LabelTargetIlRefs = labelTargetIlRefs;
            LabelNames = labelNames;
        }

        public IReadOnlyList<IlObject> IlObjects { get; }

        public IReadOnlyCollection<int> LabelTargetIlRefs { get; }

        public IReadOnlyDictionary<int, string> LabelNames { get; }
    }
}
