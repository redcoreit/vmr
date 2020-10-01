using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Assembler.Exceptions
{
    public sealed class AssemblerException : Exception
    {
        public AssemblerException(string message) : base(message)
        {
        }

        public AssemblerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
