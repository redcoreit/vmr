using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Cli.Helpers
{
    internal static class ExceptionExtensions
    {
        public static IReadOnlyList<Exception> UnwrapReverse(this Exception ex)
        {
            var stack = new Stack<Exception>();
            Exception? current = ex;

            do
            {
                stack.Push(current);
                current = current.InnerException;
            } while (current is object);

            return stack.ToArray();
        }
    }
}
