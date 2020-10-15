using System;
using System.Collections.Generic;
using System.Text;

namespace Vmr.Common.Instructions
{
    public readonly struct IlAddress
    {
        private readonly bool _initialized;
        private readonly int _segment;

        // TODO (RH Bug): change type to uint.
        public IlAddress(int segment, IlRef ilRef)
        {
            _initialized = true;
            _segment = segment;
            IlRef = ilRef;
        }

        public IlRef IlRef { get; }

        public IlAddress Segment => new IlAddress(_segment, 0);

        public int Value => _segment + IlRef.Value;

        public override bool Equals(object? obj)
            => obj is IlAddress address 
            && Equals(address);

        public bool Equals(IlAddress other)
            => _initialized == other._initialized
            && _segment == other._segment
            && IlRef == other.IlRef
            ;

        public override int GetHashCode()
            => HashCode.Combine(_initialized, _segment, IlRef);

        public static bool operator ==(IlAddress left, IlAddress right)
            => left.Equals(right);

        public static bool operator !=(IlAddress left, IlAddress right)
            => !(left == right);
    }
}
