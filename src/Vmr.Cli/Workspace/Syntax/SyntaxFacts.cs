using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Vmr.Cli.Workspace.Syntax
{
    internal static class SyntaxFacts
    {
        public static string? GetText(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.OpCode_Add => "add",
                SyntaxKind.OpCode_Br => "br",
                SyntaxKind.OpCode_Ldc_i4 => "ldc.i4",
                SyntaxKind.OpCode_Ldstr => "ldstr",
                SyntaxKind.OpCode_Pop => "pop",
                SyntaxKind.OpCode_Nop => "nop",
                SyntaxKind.OpCode_Ceq => "ceq",
                SyntaxKind.OpCode_Brfalse => "brfalse",
                SyntaxKind.OpCode_Brtrue => "brtrue",
                SyntaxKind.OpCode_Ldloc => "ldloc",
                SyntaxKind.OpCode_Stloc => "stloc",
                SyntaxKind.OpCode_Call => "call",
                SyntaxKind.OpCode_Ret => "ret",
                SyntaxKind.OpCode_Ldarg => "ldarg",
                SyntaxKind.Attribute_Entrypoint => ".entrypoint",
                SyntaxKind.Attribute_Method => ".method",
                SyntaxKind.Attribute_Locals => ".locals",
                SyntaxKind.Attribute_Args => ".args",
                _ => null, // Dynamic 
            };

        public static string GetInstructionText(SyntaxKind kind)
        {
            if (!IsInstruction(kind))
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);

            var result = GetText(kind);

            if (result is null)
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);

            return result;
        }

        public static string GetAttributeText(SyntaxKind kind)
        {
            if (!IsAttribute(kind))
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);

            var result = GetText(kind);

            if (result is null)
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);

            return result;
        }

        public static bool IsInstruction(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.OpCode_Add => true,
                SyntaxKind.OpCode_Br => true,
                SyntaxKind.OpCode_Ldc_i4 => true,
                SyntaxKind.OpCode_Ldstr => true,
                SyntaxKind.OpCode_Pop => true,
                SyntaxKind.OpCode_Nop => true,
                SyntaxKind.OpCode_Ceq => true,
                SyntaxKind.OpCode_Brfalse => true,
                SyntaxKind.OpCode_Brtrue => true,
                SyntaxKind.OpCode_Ldloc => true,
                SyntaxKind.OpCode_Stloc => true,
                SyntaxKind.OpCode_Call => true,
                SyntaxKind.OpCode_Ret => true,
                SyntaxKind.OpCode_Ldarg => true,
                _ => false,
            };

        public static bool IsAttribute(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.Attribute_Entrypoint => true,
                SyntaxKind.Attribute_Method => true,
                SyntaxKind.Attribute_Locals => true,
                SyntaxKind.Attribute_Args => true,
                _ => false,
            };
    }
}
