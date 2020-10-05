using System;

namespace Vmr.Cli.Syntax
{
    internal abstract class AbstractLexer
    {
        private readonly ReadOnlyMemory<char> _text;

        public AbstractLexer(string text)
        {
            _text = text.AsMemory();
        }

        protected SyntaxKind Kind { get; set; }

        protected object? Value { get; set; }

        protected char Current => Peek(0);

        public int Start { get; private set; }

        public int Position { get; private set; }

        protected void InitLexPhase()
        {
            Start = Position;
            Kind = SyntaxKind.BadToken;
            Value = null;
        }

        protected SyntaxToken CreateToken()
        {
            var length = Position - Start;
            var text = SliceText(Start, length);
            return new SyntaxToken(Kind, Start, new string(text), Value);
        }

        protected string SliceText(int start, int length)
            => new string(_text.Span.Slice(start, length));

        protected char Peek(int offset)
        {
            var index = Position + offset;
            if (index < _text.Length)
                return _text.Span[index];

            return '\0';
        }

        protected void MoveNext(int offset = 1)
        {
            var nextPosition = Position + offset;
            Position += offset;
        }
    }
}
