using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Cli.Helpers
{
    internal static class SpanExtensions
    {
        public static bool Contains<TItem>(this ReadOnlySpan<TItem> span, TItem item)
        {
            for (int idx = 0; idx < span.Length; idx++)
            {
                if(EqualityComparer<TItem>.Default.Equals(span[idx], item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
