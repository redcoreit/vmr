using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Cli.Syntax
{
    public struct TextSpan : IEquatable<TextSpan>
    {
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public int Start { get; } // Inclusive

        public int Length { get; }

        public int End => Start + Length; // Exclusive

        public static TextSpan FromBounds(int start, int end) => new TextSpan(start, end - start);

        public override string ToString() => $"{Start}..{End}";

        public override bool Equals(object? obj) => obj is TextSpan span && Equals(span);

        public bool Equals([AllowNull] TextSpan other) => Start == other.Start && Length == other.Length;

        public override int GetHashCode() => HashCode.Combine(Start, Length);

        public static bool operator ==(TextSpan left, TextSpan right) => left.Equals(right);

        public static bool operator !=(TextSpan left, TextSpan right) => !(left == right);
    }
}
