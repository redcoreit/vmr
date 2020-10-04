using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Vmr.Instructions
{
    public readonly ref struct IlRef
    {
        private readonly bool _initialized;
        private readonly int _ilRef;

        public IlRef(int ilRef)
        {
            _ilRef = ilRef;
            _initialized = true;
        }

        public override bool Equals(object? obj) => false;

        public bool Equals(IlRef other)
            => other._initialized == _initialized && other._ilRef == _ilRef;

        public override int GetHashCode() => HashCode.Combine(_ilRef);

        public static bool operator ==(IlRef left, IlRef right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IlRef left, IlRef right)
        {
            return !(left == right);
        }

        public override string ToString() => $"IL_{_ilRef.ToString("D4")}";

        public static implicit operator IlRef(int ilRef)
            => new IlRef(ilRef);
    }
}
