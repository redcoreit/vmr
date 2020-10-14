using System;
using System.Collections.Generic;
using System.Text;
using Vmr.Common.Exeptions;
using Vmr.Common.Instructions;

namespace Vmr.Common.Assemble
{
    internal sealed class MethodInfoBuilder : ReferenceBuilder<MethodInfo>
    {
        private int? _entrypointIlRef;
        private Dictionary<int, int> _locals;

        public MethodInfoBuilder()
        {
            _locals = new Dictionary<int, int>();
        }

        public void AddReference(IlAddress baseAddress, IlRef reference, string name)
        {
            base.AddReference(reference.ToAddress(baseAddress), name);
        }

        public void AddTarget(string name, int baseAddress, IlRef target, int locals, bool isEntryPoint)
        {
            if(isEntryPoint)
            {
                if(_entrypointIlRef.HasValue)
                {
                    throw new VmrException($"Entrypoint already specified.");
                }

                _entrypointIlRef = target.ToAddress(baseAddress);
            }

            _locals.Add(target.ToAddress(baseAddress), locals);
            base.AddTarget(name, target.ToAddress(baseAddress));
        }

        public override MethodInfo Build()
        {
            if (_entrypointIlRef is null)
            {
                throw new VmrException($"No entrypoint specified.");
            }

            return new MethodInfo(References, Targets, _entrypointIlRef.Value, _locals);
        }
    }
}
