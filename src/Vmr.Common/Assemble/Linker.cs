using System;
using System.Collections.Generic;
using System.Text;
using Vmr.Common.Primitives;

namespace Vmr.Common.Assemble
{
    public static class Linker
    {
        public static void LinkLabels(List<IlObject> program, LableInfo lableInfo)
        {
            for (var idx = 0; idx < program.Count; idx++)
            {
                var ilObj = program[idx];

                if (lableInfo.TryGetReference(ilObj.IlRef.Value, out var label))
                {
                    var value = lableInfo.GetTarget(label).Value;
                    
                    program[idx] = new IlObject(ilObj.IlRef.Value, value);
                }
            }
        }
    }
}
