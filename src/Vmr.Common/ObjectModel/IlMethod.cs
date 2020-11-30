using System;
using System.Collections.Generic;
using System.Text;

namespace Vmr.Common.ObjectModel
{
    public sealed class IlMethod : IEquatable<IlMethod?>
    {
        public IlMethod(IlAddress address, byte args, IReadOnlyList<IlObject> ilObjects)
        {
            Address = address;
            Args = args;
            IlObjects = ilObjects;
        }

        public IlAddress Address { get; }

        public byte Args { get; }

        public IReadOnlyList<IlObject> IlObjects { get; }

        public override bool Equals(object? obj)
            => Equals(obj as IlMethod);

        public bool Equals(IlMethod? other)
            => other != null
            && EqualityComparer<IlAddress>.Default.Equals(Address, other.Address)
            && Args == other.Args;

        public override int GetHashCode()
            => HashCode.Combine(Address, Args);

        public static bool operator ==(IlMethod? left, IlMethod? right)
            => left is object && left.Equals(right)
            || right is object && right.Equals(left)
            || left is null && right is null
            ;

        public static bool operator !=(IlMethod? left, IlMethod? right)
            => !(left == right);
    }
}
