using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Vmr.Common.Exeptions
{
    internal static class Throw
    {
        [DoesNotReturn]
        public static void NotSupportedInstruction(int _pointer, byte code)
           => throw new VmrException($"Not supported instruction. Address: 0x{_pointer.ToString("X4")} Code: {code.ToString("X2")}");
    }
}
