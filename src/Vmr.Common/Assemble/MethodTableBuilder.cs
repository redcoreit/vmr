using System;
using System.Collections.Generic;
using System.Text;
using Vmr.Common.Exeptions;
using Vmr.Common.Instructions;

namespace Vmr.Common.Assemble
{
    internal sealed class MethodTableBuilder : TableBuilder<MethodTable, IlAddress>
    {
        private IlAddress? _entrypointIlRef;
        private Dictionary<IlAddress, int> _locals;

        public MethodTableBuilder()
        {
            _locals = new Dictionary<IlAddress, int>();
        }

        public new void AddReference(IlAddress address, string name)
        {
            base.AddReference(address, name);
        }

        public void AddTarget(string name, IlAddress target, int locals, bool isEntryPoint)
        {
            if(isEntryPoint)
            {
                if(_entrypointIlRef.HasValue)
                {
                    throw new VmrException($"Entrypoint already specified.");
                }

                _entrypointIlRef = target;
            }

            _locals.Add(target, locals);
            base.AddTarget(name, target);
        }

        public override MethodTable Build()
        {
            if (_entrypointIlRef is null)
            {
                throw new VmrException($"No entrypoint specified.");
            }

            return new MethodTable(References, Targets, _entrypointIlRef.Value, _locals);
        }
    }
}
