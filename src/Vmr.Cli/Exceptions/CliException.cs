using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Cli.Exceptions
{
    public sealed class CliException : Exception
    {
        public CliException(string message) : base(message)
        {
        }

        public CliException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
