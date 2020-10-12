using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Workspace.Syntax.Abstraction;

namespace Vmr.Cli.Workspace.Syntax
{
    internal sealed class IlLexer : AbstractLexer
    {
        public IlLexer(string file) : base(file)
        {
        }

        internal IEnumerable<SyntaxToken> LexAll()
        {
            SyntaxToken token;
            do
            {
                token = Lex();
                yield return token;

            } while (token.Kind != SyntaxKind.EndOfFileToken);
        }

        private SyntaxToken Lex()
        {
            SkipWhitespaces();
            InitLexPhase();

            switch (Current)
            {
                case '\0':
                    {
                        Kind = SyntaxKind.EndOfFileToken;
                        break;
                    }
                case '/' when Peek(1) == '/':
                    {
                        ReadComment();
                        break;
                    }
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    {
                        ReadNumbersToken();
                        break;
                    }
                case '"':
                    {
                        ReadString();
                        break;
                    }
                default:
                    {
                        if (char.IsLetter(Current) || Current == '_')
                            ReadLiteralToken();
                        else
                        {
                            ReportBadCharacter(Position, Current);
                            MoveNext();
                        }
                        break;
                    }
            }

            return CreateToken();
        }

        private void ReadString()
        {
            // Qualified tokens text property contains the qualifiers but the value property is not!
            MoveNext();

            var done = false;
            var builder = new StringBuilder();

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        {
                            var span = new TextSpan(Start, 1);
                            ReportUnterminatedString(span);
                            done = true;
                            break;
                        }
                    case '\\':
                        {
                            if (Peek(1) == '"')
                                MoveNext();
                            builder.Append(Current);
                            MoveNext();
                            break;
                        }
                    case '"':
                        {
                            done = true;
                            MoveNext();
                            break;
                        }
                    default:
                        {
                            builder.Append(Current);
                            MoveNext();
                            break;
                        }
                }
            }

            Kind = SyntaxKind.StringToken;
            Value = builder.ToString();
        }

        private void ReadComment()
        {
            MoveNext(2);

            var done = false;
            var builder = new StringBuilder();

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        {
                            done = true;
                            break;
                        }
                    default:
                        {
                            builder.Append(Current);
                            MoveNext();
                            break;
                        }
                }
            }

            Kind = SyntaxKind.CommentToken;
            Value = builder.ToString();
        }

        private void ReadLiteralToken()
        {
            do
            {
                MoveNext();
            } while (!char.IsWhiteSpace(Current) && Current != '\0');

            var literals = SliceText(Start, Position - Start).ToLowerInvariant();

            // We try to figure out that the current text is a keyword or just a literal.
            // The order of cases is important because the way the LALR parsing works.
            Kind = true switch
            {
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Brfalse)) => SyntaxKind.OpCode_Brfalse,
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Brtrue)) => SyntaxKind.OpCode_Brtrue,
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Add)) => SyntaxKind.OpCode_Add,
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Br)) => SyntaxKind.OpCode_Br,
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Ldc_i4)) => SyntaxKind.OpCode_Ldc_i4,
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Ldstr)) => SyntaxKind.OpCode_Ldstr,
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Nop)) => SyntaxKind.OpCode_Nop,
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Pop)) => SyntaxKind.OpCode_Pop,
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Ceq)) => SyntaxKind.OpCode_Ceq,
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Ldloc)) => SyntaxKind.OpCode_Ldloc,
                true when Match(SyntaxFacts.GetInstructionText(SyntaxKind.OpCode_Stloc)) => SyntaxKind.OpCode_Stloc,
                _ => SyntaxKind.LiteralToken,
            };

            if (Kind == SyntaxKind.LiteralToken)
            {
                if (literals.AsSpan()[^1] == ':')
                {
                    Kind = SyntaxKind.LabelDeclarationToken;
                    Value = literals.TrimEnd(':');
                }
                else
                {
                    Value = literals;
                }
            }

            bool Match(params string[] words)
            {
                if (words.Length == 0)
                    throw new CliException("Match method call must have at least one parameter.");

                if (!string.Equals(literals, words[0]))
                    return false;

                var nextLiteralStartIndex = 0;
                for (var wordIndex = 1; wordIndex < words.Length; wordIndex++)
                {
                    var word = words[wordIndex];

                    // Skip all whitespaces.
                    // Multipart keywords may have whitespaces between parts therefore we use while-do construct.
                    while (char.IsWhiteSpace(Peek(nextLiteralStartIndex)))
                        nextLiteralStartIndex++;

                    for (var i = 0; i < word.Length; i++)
                    {
                        if (!Equals(Peek(nextLiteralStartIndex + i), word[i]))
                            return false;
                    }

                    // Adding length instead of length -1 we implicitly starts the next iteration from the correct (next unprocessed) char.
                    nextLiteralStartIndex += word.Length;
                }

                MoveNext(nextLiteralStartIndex);
                return true;
            }
        }

        private void ReadNumbersToken()
        {
            var decimalSeparatorFound = false;

            do
            {
                MoveNext();
                if (!decimalSeparatorFound && Current == '.' && char.IsDigit(Peek(1)))
                {
                    decimalSeparatorFound = true;
                    MoveNext();
                }
            } while (char.IsDigit(Current));

            Kind = decimalSeparatorFound
                ? SyntaxKind.DecimalToken
                : SyntaxKind.Int32Token;

            var text = SliceText(Start, Position - Start);

            object value = Kind switch
            {
                SyntaxKind.DecimalToken => decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var num) ? num : null,
                SyntaxKind.Int32Token => int.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var num) ? num : null,
                _ => throw new ArgumentOutOfRangeException(nameof(Kind), Kind, null)
            };

            if (value is null)
                ReportInvalidNumber(new TextSpan(Start, Position - Start), text);

            Value = value;
        }

        private void SkipWhitespaces()
        {
            if (Position == 0 && Current == Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble())[0])
            {
                MoveNext(3);
            }

            // Vml lexer intentionally does not emits whitespace tokens.
            while (char.IsWhiteSpace(Current))
            {
                MoveNext();
            }
        }

        private void ReportBadCharacter(int position, char current)
            => Console.WriteLine($"Bad character '{current}' in position {position}.");

        private void ReportUnterminatedString(TextSpan span)
            => Console.WriteLine($"Unterminated string detected at position {span}.");

        private void ReportInvalidNumber(TextSpan span, string text)
            => Console.WriteLine($"Invalid number '{text}' detected at position {span}.");
    }
}
