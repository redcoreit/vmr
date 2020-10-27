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
        internal IlProgram(IlAddress entryPoint,
                           IReadOnlyList<IlMethod> ilMethods,
                           IReadOnlyCollection<IlAddress> labelTargets)
            : this(entryPoint, ilMethods, labelTargets, new Dictionary<IlAddress, string>(), new Dictionary<IlAddress, string>(), new Dictionary<IlAddress, List<string>>())
        {
        }

        internal IlProgram(IlAddress entryPoint,
                           IReadOnlyList<IlMethod> ilMethods,
                           IReadOnlyCollection<IlAddress> labelTargets,
                           IReadOnlyDictionary<IlAddress, string> labelNames,
                           IReadOnlyDictionary<IlAddress, string> methodNames,
                           IReadOnlyDictionary<IlAddress, List<string>> comments)
        {
            EntryPoint = entryPoint;
            IlMethods = ilMethods;
            LabelTargets = labelTargets;
            LabelNames = labelNames;
            MethodNames = methodNames;
            Comments = comments;
        }

        public IlAddress EntryPoint { get; }

        public IReadOnlyList<IlMethod> IlMethods { get; }

        public IReadOnlyCollection<IlAddress> LabelTargets { get; }

        public IReadOnlyDictionary<IlAddress, string> LabelNames { get; }
        
        public IReadOnlyDictionary<IlAddress, string> MethodNames { get; }
        
        public IReadOnlyDictionary<IlAddress, List<string>> Comments { get; }
    }
}
