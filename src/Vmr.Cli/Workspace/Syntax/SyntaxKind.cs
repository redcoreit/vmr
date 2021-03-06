﻿using System.Text;

namespace Vmr.Cli.Workspace.Syntax
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
        OpCode_Brfalse,
        OpCode_Brtrue,
        OpCode_Ldloc,
        OpCode_Stloc,
        OpCode_Call,
        OpCode_Ret,
        OpCode_Ldarg,

        // Attributes
        Attribute_Method,
        Attribute_Entrypoint,
        Attribute_Locals,
        Attribute_Args,
    }
}
