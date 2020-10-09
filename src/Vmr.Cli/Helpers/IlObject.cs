using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Instructions;

namespace Vmr.Cli.Helpers
{
    internal sealed record IlObject
    {
        private readonly int _ilRef;

        public IlObject(int ilRef, object obj)
        {
            _ilRef = ilRef;
            Obj = obj;
        }

        public IlRef IlRef => _ilRef;

        public object Obj { get; init; }
    }
}
