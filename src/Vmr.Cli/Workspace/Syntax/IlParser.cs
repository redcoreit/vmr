using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Workspace.Syntax.Abstraction;
using Vmr.Common;

namespace Vmr.Cli.Workspace.Syntax
{
    internal sealed class IlParser : AbstractParser
    {
        private readonly CodeBuilder _codeBuilder;

        public IlParser(string text) : base(text)
        {
            _codeBuilder = new CodeBuilder();
        }

        public CodeBuilder Parse()
        {
            while (Current.Kind != SyntaxKind.EndOfFileToken)
            {
                ParsePrimary();
            }

            return _codeBuilder;
        }

        private void ParsePrimary()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.LabelDeclarationToken:
                    {
                        ParseLabel();
                        break;
                    }
                case SyntaxKind.Attribute_Method:
                    {
                        ParseMethodDeclaration();
                        break;
                    }
                case SyntaxKind.CommentToken:
                    {
                        ParseComment();
                        break;
                    }
                default:
                    {
                        ParseInstruction();
                        break;
                    }
            }
        }

        private void ParseLabel()
        {
            var label = ExpectToken(SyntaxKind.LabelDeclarationToken);
            _codeBuilder.Label(label.Value!.ToString()!);
        }

        private void ParseMethodDeclaration()
        {
            var attribute = ExpectToken(SyntaxKind.Attribute_Method);
            var name = ExpectToken(SyntaxKind.LabelDeclarationToken);
            var isEntrypoint = Current.Kind == SyntaxKind.Attribute_Entrypoint;

            if (isEntrypoint)
            {
                ReadAndMoveNext();
            }

            var locals = 0;

            if (Current.Kind == SyntaxKind.Attribute_Locals)
            {
                ReadAndMoveNext();
                var value = ExpectToken(SyntaxKind.Int32Token);
                locals = (int)value.Value!;
            }

            _codeBuilder.Method(name.Value!.ToString()!, locals, isEntrypoint);
        }

        private void ParseComment()
        {
            var comment = ExpectToken(SyntaxKind.CommentToken);
            _codeBuilder.Comment(comment.Value!.ToString()!);
        }

        private void ParseInstruction()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpCode_Add:
                    {
                        ParseAdd();
                        break;
                    }
                case SyntaxKind.OpCode_Ldc_i4:
                    {
                        ParseLdc_i4();
                        break;
                    }
                case SyntaxKind.OpCode_Ldstr:
                    {
                        ParseLdstr();
                        break;
                    }
                case SyntaxKind.OpCode_Pop:
                    {
                        ParsePop();
                        break;
                    }
                case SyntaxKind.OpCode_Br:
                    {
                        ParseBr();
                        break;
                    }
                case SyntaxKind.OpCode_Brtrue:
                    {
                        ParseBrtrue();
                        break;
                    }
                case SyntaxKind.OpCode_Brfalse:
                    {
                        ParseBrfalse();
                        break;
                    }
                case SyntaxKind.OpCode_Ceq:
                    {
                        ParseCeq();
                        break;
                    }
                case SyntaxKind.OpCode_Ldloc:
                    {
                        ParseLdloc();
                        break;
                    }
                case SyntaxKind.OpCode_Stloc:
                    {
                        ParseStloc();
                        break;
                    }
                case SyntaxKind.OpCode_Call:
                    {
                        ParseCall();
                        break;
                    }
                case SyntaxKind.OpCode_Ret:
                    {
                        ParseRet();
                        break;
                    }
                case SyntaxKind.OpCode_Nop:
                default:
                    {
                        ParseNop();
                        break;
                    }
            }
        }

        private void ParseAdd()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Add);
            _codeBuilder.Add();
        }

        private void ParseLdc_i4()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Ldc_i4);
            var arg = ExpectToken(SyntaxKind.Int32Token);
            _codeBuilder.Ldc_i4((int)arg.Value!);
        }

        private void ParseLdstr()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Ldstr);
            var arg = ExpectToken(SyntaxKind.StringToken);
            _codeBuilder.Ldstr(arg.Value!.ToString()!);
        }

        private void ParsePop()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Pop);
            _codeBuilder.Pop();
        }

        private void ParseBr()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Br);
            var arg = ExpectToken(SyntaxKind.LiteralToken);
            _codeBuilder.Br(arg.Value!.ToString()!);
        }

        private void ParseNop()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Nop);
            _codeBuilder.Nop();
        }

        private void ParseCeq()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Ceq);
            _codeBuilder.Ceq();
        }

        private void ParseBrfalse()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Brfalse);
            var arg = ExpectToken(SyntaxKind.LiteralToken);
            _codeBuilder.Brfalse(arg.Text);
        }

        private void ParseBrtrue()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Brtrue);
            var arg = ExpectToken(SyntaxKind.LiteralToken);
            _codeBuilder.Brtrue(arg.Value!.ToString()!);
        }

        private void ParseLdloc()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Ldloc);
            var arg = ExpectToken(SyntaxKind.Int32Token);
            _codeBuilder.Ldloc((int)arg.Value!);
        }

        private void ParseStloc()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Stloc);
            var arg = ExpectToken(SyntaxKind.Int32Token);
            _codeBuilder.Stloc((int)arg.Value!);
        }

        private void ParseCall()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Call);
            var arg = ExpectToken(SyntaxKind.LiteralToken);
            _codeBuilder.Call(arg.Text);
        }

        private void ParseRet()
        {
            var op = ExpectToken(SyntaxKind.OpCode_Ret);
            _codeBuilder.Ret();
        }
    }
}
