using System;
using System.Collections.Generic;
using System.Text;

namespace Vmr.Common.Primitives
{
    public sealed class IlMethod : IEquatable<IlMethod?>
    {
        public IlMethod(IlAddress address, uint size, IReadOnlyList<IlObject> ilObjects)
        {
            Address = address;
            Size = size;
            IlObjects = ilObjects;
        }

        public IlAddress Address { get; }

        // TODO (RH -): remove
        public uint Size { get; }

        public IReadOnlyList<IlObject> IlObjects { get; }

        public override bool Equals(object? obj)
            => Equals(obj as IlMethod);

        public bool Equals(IlMethod? other)
            => other != null
            && EqualityComparer<IlAddress>.Default.Equals(Address, other.Address)
            && Size == other.Size;

        public override int GetHashCode()
            => HashCode.Combine(Address, Size);

        public static bool operator ==(IlMethod? left, IlMethod? right)
            => left is object && left.Equals(right)
            || right is object && right.Equals(left)
            || left is null && right is null
            ;

        public static bool operator !=(IlMethod? left, IlMethod? right) 
            => !(left == right);
    }
}
