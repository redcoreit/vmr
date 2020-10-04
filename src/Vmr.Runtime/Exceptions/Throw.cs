using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Instructions;

namespace Vmr.Runtime.Exceptions
{
    internal static class Throw
    {
        [DoesNotReturn]
        public static void MissingInstructionArgument(IlRef ilRef)
            => throw new VmExecutionException($"Required instruction argument missing. InstructionIndex: {ilRef.ToString()}");

        [DoesNotReturn]
        public static void NotSupportedInstruction(IlRef ilRef)
            => throw new VmExecutionException($"Required instruction argument missing. InstructionIndex: {ilRef.ToString()}");

        [DoesNotReturn]
        internal static void OperationNotSupported(InstructionCode instructionCode)
            => OperationNotSupported(instructionCode, Array.Empty<object>());

        [DoesNotReturn]
        internal static void OperationNotSupported(InstructionCode instructionCode, params object[] args)
            => throw new VmExecutionException($"Operation is not supported. InstructionCode: {InstructionFacts.Format(instructionCode)} Args: '{string.Join(' ', args)}'.");

        [DoesNotReturn]
        public static void StackUnderflowException(IlRef ilRef)
            => throw new VmExecutionException($"Stack underflow exception. IlRef: {ilRef.ToString()}");
    }
}
