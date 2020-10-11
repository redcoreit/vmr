using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Common.Exeptions
{
    public sealed class VmrException : Exception
    {
        public VmrException(string message) : base(message)
        {
        }

        public VmrException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
