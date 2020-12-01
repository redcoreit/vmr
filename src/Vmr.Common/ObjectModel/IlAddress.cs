using System;
using System.Collections.Generic;
using System.Text;

namespace Vmr.Common.ObjectModel
{
    public sealed class IlAddress
        : IEquatable<IlAddress?>
        , IComparable<IlAddress>
    {
        private readonly bool _initialized;
        private readonly int _address;

        public IlAddress(int address)
        {
            _initialized = true;
            _address = address;
        }

        public int Value => _address;

        public override string ToString()
            => $"0x{Value.ToString("X4")}";

        public override bool Equals(object? obj)
            => Equals(obj as IlAddress);

        public bool Equals(IlAddress? other)
            => other != null
            && _initialized == other._initialized
            && _address == other._address
            ;

        public override int GetHashCode()
            => HashCode.Combine(_initialized, _address);

        public int CompareTo(IlAddress other)
            => _address.CompareTo(other._address);

        public static bool operator ==(IlAddress? left, IlAddress? right)
            => left is object && left.Equals(right)
            || right is object && right.Equals(left)
            || left is null && right is null
            ;

        public static bool operator !=(IlAddress? left, IlAddress? right)
            => !(left == right);


        // TODO (RH -): Do not allocate on explicit equality check with int value.
        public static implicit operator IlAddress(int value)
            => new IlAddress(value);
    }
}
