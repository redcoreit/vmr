using System;
using System.Collections.Generic;
using System.Text;

namespace Vmr.Common.Primitives
{
    internal class Comment : ProgramNode
    {
        public Comment(string text) 
            => Text = text;

        public override int Size => 0;

        public string Text { get; }
    }
}
