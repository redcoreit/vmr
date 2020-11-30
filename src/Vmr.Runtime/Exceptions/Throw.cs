using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common.Instructions;
using Vmr.Common.ObjectModel;

namespace Vmr.Runtime.Exceptions
{
    internal static class Throw
    {
        [DoesNotReturn]
        public static void InvalidInstructionArgument(IlAddress ilAddress)
            => throw new VmExecutionException($"Invalid instruction argument. Address: {ilAddress.ToString()}");

        [DoesNotReturn]
        public static void MissingInstructionArgument(IlAddress ilAddress)
            => throw new VmExecutionException($"Required instruction argument missing. Address: {ilAddress.ToString()}");

        [DoesNotReturn]
        public static void NotSupportedInstruction(IlAddress ilAddress, byte code)
            => throw new VmExecutionException($"Not supported instruction. Address: {ilAddress.ToString()} Code: {InstructionFacts.Format(code)}");

        [DoesNotReturn]
        internal static void OperationNotSupported(InstructionCode instructionCode)
            => OperationNotSupported(instructionCode, Array.Empty<object>());

        [DoesNotReturn]
        internal static void OperationNotSupported(InstructionCode instructionCode, params object[] args)
            => throw new VmExecutionException($"Operation is not supported. InstructionCode: {InstructionFacts.Format(instructionCode)} Args: '{string.Join(", ", args)}'.");

        [DoesNotReturn]
        public static void StackUnderflowException(IlAddress ilAddress)
            => throw new VmExecutionException($"Stack underflow exception. Address: {ilAddress.ToString()}");
        
        [DoesNotReturn]
        public static void LocalVariableNotSet(IlAddress ilAddress)
            => throw new VmExecutionException($"Local variable not set. Address: {ilAddress.ToString()}");

        [DoesNotReturn]
        public static void ArgumentNotFound(IlAddress ilAddress)
            => throw new VmExecutionException($"Argument not found. Address: {ilAddress.ToString()}");


        [DoesNotReturn]
        public static void InvalidStackSize(IlAddress ilAddress, int size)
            => throw new VmExecutionException($"Return failed because of invalid stack size '{size}'. Address: {ilAddress.ToString()}");

    }
}
