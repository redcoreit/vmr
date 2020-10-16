using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Runtime.Exceptions
{
    internal static class Throw
    {
        [DoesNotReturn]
        public static void InvalidInstructionArgument(IlAddress ilAddress)
            => throw new VmExecutionException($"Invalid instruction argument. IlRef: {ilAddress.ToString()}");

        [DoesNotReturn]
        public static void MissingInstructionArgument(IlAddress ilAddress)
            => throw new VmExecutionException($"Required instruction argument missing. IlRef: {ilAddress.ToString()}");

        [DoesNotReturn]
        public static void NotSupportedInstruction(IlAddress ilAddress, byte code)
            => throw new VmExecutionException($"Not supported instruction. IlRef: {ilAddress.ToString()} Code: {InstructionFacts.Format(code)}");

        [DoesNotReturn]
        internal static void OperationNotSupported(InstructionCode instructionCode)
            => OperationNotSupported(instructionCode, Array.Empty<object>());

        [DoesNotReturn]
        internal static void OperationNotSupported(InstructionCode instructionCode, params object[] args)
            => throw new VmExecutionException($"Operation is not supported. InstructionCode: {InstructionFacts.Format(instructionCode)} Args: '{string.Join(", ", args)}'.");

        [DoesNotReturn]
        public static void StackUnderflowException(IlAddress ilAddress)
            => throw new VmExecutionException($"Stack underflow exception. IlRef: {ilAddress.ToString()}");
        
        [DoesNotReturn]
        public static void LocalVariableNotSet(IlAddress ilAddress)
            => throw new VmExecutionException($"Local variable not set. IlRef: {ilAddress.ToString()}");
    }
}
