using System;
using System.Collections.Generic;
using System.Text;

namespace Vmr.Common.Helpers
{
    public static class ArrayExtensions
    {
        public static ReadOnlySpan<TItem> AsReadOnlySpan<TItem>(this TItem[] array)
            => array.AsSpan();
    }
}
