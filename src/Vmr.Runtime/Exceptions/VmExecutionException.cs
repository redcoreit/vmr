using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Runtime.Exceptions
{
    public sealed class VmExecutionException : Exception
    {
        public VmExecutionException(string message) : base(message)
        {
        }

        public VmExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
