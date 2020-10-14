#pragma warning disable CA1707

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Common.Instructions
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
        Ldloc = 0x8E,
        Stloc = 0x51,
        Call = 0x28,
        Ret = 0x2A,
    }
}
