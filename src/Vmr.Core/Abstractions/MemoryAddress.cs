using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vmr.Core.Abstractions
{
    internal readonly struct MemoryAddress : IEquatable<MemoryAddress>
    {
        private readonly bool _isConstructed;
        private readonly int _address;

        public MemoryAddress(int address)
        {
            _isConstructed = true;
            _address = address;
        }

        public static implicit operator int(MemoryAddress memoryAddress)
            => memoryAddress._address;

        public static bool operator ==(MemoryAddress left, MemoryAddress right) 
            => left.Equals(right);

        public static bool operator !=(MemoryAddress left, MemoryAddress right) 
            => !(left == right);

        public override bool Equals(object? obj) 
            => obj is MemoryAddress address && Equals(address);
        
        public bool Equals(MemoryAddress other) 
            => _isConstructed == other._isConstructed && _address == other._address;
        
        public override int GetHashCode() 
            => HashCode.Combine(_isConstructed, _address);

        public override string ToString() => _address.ToString("X8");
    }
}
