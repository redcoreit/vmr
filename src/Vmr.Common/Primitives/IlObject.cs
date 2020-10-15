using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common.Instructions;

namespace Vmr.Common.Primitives
{
    public sealed class IlObject : IEquatable<IlObject?>
    {
        internal IlObject(IlAddress address, object obj)
        {
            Address = address;
            Obj = obj;
        }
        
        public IlAddress Address { get; }

        public object Obj { get; }

        public override bool Equals(object? obj)
            => Equals(obj as IlObject);

        public bool Equals(IlObject? other)
            => other != null
            && Address == other.Address
            && EqualityComparer<object>.Default.Equals(Obj, other.Obj);

        public override int GetHashCode()
            => HashCode.Combine(Address, Obj);

        public static bool operator ==(IlObject? left, IlObject? right)
            => left is null && right is null
            || (left?.Equals(right) ?? false);

        public static bool operator !=(IlObject? left, IlObject? right)
            => !(left == right);
    }
}
