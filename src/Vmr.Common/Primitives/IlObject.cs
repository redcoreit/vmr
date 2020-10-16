﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Common.Primitives
{
    public sealed class IlObject : IEquatable<IlObject?>
    {
        internal IlObject(IlAddress address, uint size, object obj)
        {
            Address = address;
            Size = size;
            Obj = obj;
        }

        public IlAddress Address { get; }

        // TODO (RH -): remove
        public uint Size { get; }

        public object Obj { get; }

        public override bool Equals(object? obj)
            => Equals(obj as IlObject);

        public bool Equals(IlObject? other)
            => other != null
            && Address == other.Address
            && Size == other.Size
            && EqualityComparer<object>.Default.Equals(Obj, other.Obj);

        public override int GetHashCode()
            => HashCode.Combine(Size, Obj);

        public static bool operator ==(IlObject? left, IlObject? right)
            => left is object && left.Equals(right)
            || right is object && right.Equals(left)
            || left is null && right is null
            ;

        public static bool operator !=(IlObject? left, IlObject? right)
            => !(left == right);
    }
}
