using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Vmr.Instructions
{
    public readonly ref struct IlRef
    {
        private readonly bool _initialized;
        private readonly int _address;

        public IlRef(int address)
        {
            _address = address;
            _initialized = true;
        }

        public int Address => _address;

        public override bool Equals(object? obj) => false;

        public bool Equals(IlRef other)
            => other._initialized == _initialized && other._address == _address;

        public override int GetHashCode() => HashCode.Combine(_address);

        public static bool operator ==(IlRef left, IlRef right) 
            => left.Equals(right);

        public static bool operator !=(IlRef left, IlRef right) 
            => !(left == right);

        public override string ToString() 
            => $"IL_{_address.ToString("D4")}";

        public static implicit operator IlRef(int ilRef)
            => new IlRef(ilRef);
    }
}
