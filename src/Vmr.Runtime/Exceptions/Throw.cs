using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common.Instructions;

namespace Vmr.Runtime.Exceptions
{
    internal static class Throw
    {
        [DoesNotReturn]
        public static void InvalidInstructionArgument(IlRef ilRef)
            => throw new VmExecutionException($"Invalid instruction argument. IlRef: {ilRef.ToString()}");

        [DoesNotReturn]
        public static void MissingInstructionArgument(IlRef ilRef)
            => throw new VmExecutionException($"Required instruction argument missing. IlRef: {ilRef.ToString()}");

        [DoesNotReturn]
        public static void NotSupportedInstruction(IlRef ilRef, byte code)
            => throw new VmExecutionException($"Not supported instruction. IlRef: {ilRef.ToString()} Code: {InstructionFacts.Format(code)}");

        [DoesNotReturn]
        internal static void OperationNotSupported(InstructionCode instructionCode)
            => OperationNotSupported(instructionCode, Array.Empty<object>());

        [DoesNotReturn]
        internal static void OperationNotSupported(InstructionCode instructionCode, params object[] args)
            => throw new VmExecutionException($"Operation is not supported. InstructionCode: {InstructionFacts.Format(instructionCode)} Args: '{string.Join(", ", args)}'.");

        [DoesNotReturn]
        public static void StackUnderflowException(IlRef ilRef)
            => throw new VmExecutionException($"Stack underflow exception. IlRef: {ilRef.ToString()}");
        
        [DoesNotReturn]
        public static void LocalVariableNotSet(IlRef ilRef)
            => throw new VmExecutionException($"Local variable not set. IlRef: {ilRef.ToString()}");
    }
}
