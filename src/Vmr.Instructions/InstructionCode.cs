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
        //Ldfld <field> = 0x8F,
        //Ldflda <field> = 0x7B,
    }
}
