using System;
using System.Collections.Generic;
using System.Text;
using Vmr.Common.Instructions;

namespace Vmr.Common.Assemble
{
    internal sealed class LabelTableBuilder : TableBuilder<LabelTable, IlAddress>
    {
        public new void AddReference(IlAddress reference, string name)
            => base.AddReference(reference, name);

        public new void AddTarget(string name, IlAddress target)
            => base.AddTarget(name, target);

        public override LabelTable Build()
            => new LabelTable(References, Targets);
    }
}
