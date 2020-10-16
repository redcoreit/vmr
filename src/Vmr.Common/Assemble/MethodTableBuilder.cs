using System;
using System.Collections.Generic;
using System.Text;
using Vmr.Common.Exeptions;
using Vmr.Common.Primitives;

namespace Vmr.Common.Assemble
{
    internal sealed class MethodTableBuilder : TableBuilder<MethodTable>
    {
        private Dictionary<IlAddress, int> _locals;

        public MethodTableBuilder()
        {
            _locals = new Dictionary<IlAddress, int>();
        }

        public new void AddReference(IlAddress address, string name)
        {
            base.AddReference(address, name);
        }

        public void AddTarget(string name, IlAddress target, int locals)
        {
            _locals.Add(target, locals);
            base.AddTarget(name, target);
        }

        public override MethodTable Build()
            => new MethodTable(References, Targets, _locals);
    }
}
