﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.Helpers;

namespace Vmr.Cli.Workspace.Syntax.Abstraction
{
    internal abstract class AbstractParser
    {
        private readonly IReadOnlyList<SyntaxToken> _tokens;

        private int _position;

        public AbstractParser(string text)
        {
            var lexer = new IlLexer(text);
            _tokens = lexer.LexAll().Where(FilterTokenKind).ToArray();

            static bool FilterTokenKind(SyntaxToken token)
            {
                switch (token.Kind)
                {
                    case SyntaxKind.BadToken:
                        return false;

                }

                return true;
            }
        }


        protected SyntaxToken Current => Peek(0);

        protected SyntaxToken ExpectToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return ReadAndMoveNext();

            ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            var fakeToken = new SyntaxToken(kind, TextSpan.FromBounds(_position, 0));
            ReadAndMoveNext();

            return fakeToken;
        }

        protected SyntaxToken ExpectToken(params SyntaxKind[] kinds)
            => ExpectToken(kinds);

        protected SyntaxToken ExpectToken(ReadOnlySpan<SyntaxKind> kinds)
        {
            if (kinds.IsEmpty)
                throw new InvalidOperationException("At least one parameter should be specified.");

            if (kinds.Contains(Current.Kind))
                return ReadAndMoveNext();

            var primeTokenKind = kinds[0];

            ReportUnexpectedToken(Current.Span, Current.Kind, primeTokenKind);
            var fakeToken = new SyntaxToken(primeTokenKind, TextSpan.FromBounds(_position, 0));
            ReadAndMoveNext();

            return fakeToken;
        }

        protected SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index < _tokens.Count)
                return _tokens[index];

            return _tokens.Last();
        }

        protected SyntaxToken ReadAndMoveNext()
        {
            var current = Current;
            _position++;
            return current;
        }

        private void ReportUnexpectedToken(TextSpan span, SyntaxKind actual, SyntaxKind expected)
            => Console.WriteLine($"Token '{expected}' expected but '{actual}' found at {span.ToString()}.");
    }
}
