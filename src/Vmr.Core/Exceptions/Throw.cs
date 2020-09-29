using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Core.Abstractions;

namespace Vmr.Core.Exceptions
{
    internal static class Throw
    {
        [DoesNotReturn]
        public static void MissingInstructionArgument(InstructionCode instructionCode)
            => throw new VmExecutionException($"Required instruction argument missing. InstructionCode: {InstructionFacts.Format(instructionCode)}");

        [DoesNotReturn]
        internal static void OperationNotSupported(InstructionCode instructionCode)
            => OperationNotSupported(instructionCode, Array.Empty<object>());

        [DoesNotReturn]
        internal static void OperationNotSupported(InstructionCode instructionCode, params object[] args)
            => throw new VmExecutionException($"Operation is not supported. InstructionCode: {InstructionFacts.Format(instructionCode)} Args: '{string.Join(' ',args)}'.");

        [DoesNotReturn]
        public static void StackUnderflowException(InstructionCode instructionCode)
            => throw new VmExecutionException($"Stack underflow exception. InstructionCode: {InstructionFacts.Format(instructionCode)}");
    }
}
