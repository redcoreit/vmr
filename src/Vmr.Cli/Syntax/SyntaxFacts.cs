using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Vmr.Cli.Syntax
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
                _ => null, // Dynamic 
            };

        public static string GetInstructionText(SyntaxKind kind)
        {
            if(!IsInstruction(kind))
            {
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

            var result = GetText(kind);

            if(result is null)
            {
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

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
                _ => false,
            };
    }
}
