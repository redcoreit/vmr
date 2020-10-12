using System;
using System.Collections.Generic;
using System.Linq;

namespace Vmr.Cli.Workspace.Syntax
{
    internal sealed class SyntaxToken
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object? value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
            Span = new TextSpan(position, text?.Length ?? 0);
        }

        public SyntaxToken(SyntaxKind kind, TextSpan span, string text, object? value)
        {
            Kind = kind;
            Position = span.Start;
            Text = text;
            Value = value;
            Span = span;
        }

        public SyntaxToken(SyntaxKind kind, TextSpan span)
        {
            Kind = kind;
            Position = span.Start;
            Text = string.Empty;
            Value = null;
            Span = span;
        }
        public TextSpan Span { get; }

        public SyntaxKind Kind { get; }

        public int Position { get; }

        public string Text { get; }

        public object? Value { get; }
    }
}
