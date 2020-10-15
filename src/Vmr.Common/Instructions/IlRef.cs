﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Vmr.Common.Instructions
{
    public readonly struct IlRef
    {
        private readonly bool _initialized;
        private readonly int _value;

        // TODO (RH Bug): change type to uint.
        public IlRef(int ilRef)
        {
            _value = ilRef;
            _initialized = true;
        }

        public int Value => _value;

        public override bool Equals(object? obj)
            => obj is IlRef ilRef
            && Equals(ilRef);

        public bool Equals(IlRef other)
            => other._initialized == _initialized 
            && other._value == _value
            ;

        public override int GetHashCode() 
            => HashCode.Combine(_value);

        public static bool operator ==(IlRef left, IlRef right)
            => left.Equals(right);

        public static bool operator !=(IlRef left, IlRef right)
            => !(left == right);

        public override string ToString()
            => $"IL_{_value.ToString("X4")}";

        public static implicit operator IlRef(int ilRef)
            => new IlRef(ilRef);
    }
}
