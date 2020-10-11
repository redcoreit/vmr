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
        {
            IlObjects = ilObjects;
            LabelTargetIlRefs = labelTargetIlRefs;
        }

        public IReadOnlyList<IlObject> IlObjects { get; }

        public IReadOnlyCollection<int> LabelTargetIlRefs { get; }
    }
}
