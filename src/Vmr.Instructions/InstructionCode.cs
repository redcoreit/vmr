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
        Ldc = 0x0F,
        Pop = 0x60,
    }
}
