using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Cli.Helpers
{
    internal sealed record DasmProgram(IReadOnlyList<IlObject> IlObjects, IReadOnlyCollection<int> LabelTargetIlRefs);
}
