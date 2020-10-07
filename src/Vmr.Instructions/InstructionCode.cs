using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Instructions
{
    public enum InstructionCode : byte
    {
        Add = 0x58,
        Ldc_i4 = 0x0F,
        Ldstr = 0x7F,
        Pop = 0x60,
        Br = 0x38,
        Nop = 0xff, // Nop has no value, we made one.
        Ceq = 0x74,
        Brfalse = 0x39,
        Brtrue = 0x3A,
        //_Locals = 
        //ldloc_s <int> = 0x09,
        //stloc_s <int> = 0x0D,
        //Ldfld <field> = 0x8F,
        //Ldflda <field> = 0x7B,
    }
}
