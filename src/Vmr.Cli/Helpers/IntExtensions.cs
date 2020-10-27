using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Cli.Helpers
{
    internal static class IntExtensions
    {
        public static string ToIlRef(this int value)
            => $"IL_{value.ToString("X2")}";
    }
}
