using System.Text;

namespace Vmr.Cli.Syntax
{
    internal enum SyntaxKind
    {
        //Tokens
        BadToken = 1,
        EndOfFileToken,
        LiteralToken,
        LabelDeclarationToken,
        StringToken,
        CommentToken,
        Int32Token,
        DecimalToken,

        // Instructions
        OpCode_Add,
        OpCode_Ldc_i4,
        OpCode_Ldstr,
        OpCode_Pop,
        OpCode_Br,
        OpCode_Nop,
        OpCode_Ceq,
    }
}
