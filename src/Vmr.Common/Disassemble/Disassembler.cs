using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Transactions;

using Vmr.Common;
using Vmr.Common.Exeptions;
using Vmr.Common.Helpers;
using Vmr.Common.Instructions;
using Vmr.Common.Linking;
using Vmr.Common.ObjectModel;

namespace Vmr.Common.Disassemble
{
    public class Disassembler
    {
        private readonly HashSet<IlAddress> _labelTargets;

        private Disassembler()
        {
            _labelTargets = new HashSet<IlAddress>();
        }

        public static IlProgram GetProgram(byte[] program)
        {
            var instance = new Disassembler();
            var span = program.AsSpan();
            return instance.GetProgram(span);
        }

        private IlProgram GetProgram(ReadOnlySpan<byte> program)
        {
            var pointer = 0;
            var ilMethods = new List<IlMethod>();
            var methodNames = new Dictionary<IlAddress, string>();

            var entryPointValue = BinaryConvert.GetInt32(ref pointer, program);

            var callTargets = new Stack<int>();
            callTargets.Push(entryPointValue);
            var visitedTargets = new HashSet<int> { entryPointValue };

            while (callTargets.Count > 0)
            {
                var ilObjects = new List<IlObject>();

                pointer = callTargets.Pop();
                var argCount = program[pointer];

                var method = new IlMethod(pointer, argCount, ilObjects);
                ilMethods.Add(method);
                methodNames.Add(method.Address, $"m{ilMethods.Count}");

                pointer += InstructionFacts.SizeOfMethodHeader;

                while (pointer < program.Length)
                {
                    var instructionAddress = pointer;
                    var instruction = GetInstruction(program, ref pointer);
                    ilObjects.Add(new IlObject(instructionAddress, 0, instruction));

                    var arg = ReadArguments(instruction, program, ref pointer);

                    if (instruction == InstructionCode.Ret)
                    {
                        break;
                    }

                    if (arg is null)
                    {
                        continue;
                    }

                    ilObjects.Add(arg);

                    if (instruction != InstructionCode.Call)
                    {
                        continue;
                    }

                    var callTarget = (int)arg.Obj;

                    if (!visitedTargets.Add(callTarget))
                    {
                        continue;
                    }

                    callTargets.Push(callTarget);
                }
            }

            return new IlProgram(entryPointValue, ilMethods.OrderBy(m => m.Address).ToArray(), methodNames, _labelTargets);
        }

        private InstructionCode GetInstruction(ReadOnlySpan<byte> program, ref int pointer)
        {
            try
            {
                return BinaryConvert.GetInstructionCode(ref pointer, program);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex.Message);
                Throw.NotSupportedInstruction(pointer, program[pointer]);
                throw;
            }
        }

        private IlObject? ReadArguments(InstructionCode instruction, ReadOnlySpan<byte> program, ref int pointer)
        {
            var address = new IlAddress(pointer);

            switch (instruction)
            {
                case InstructionCode.Ldstr:
                    {
                        var arg = BinaryConvert.GetString(ref pointer, program);
                        return new IlObject(address, 0, arg);
                    }
                case InstructionCode.Ldloc:
                case InstructionCode.Stloc:
                case InstructionCode.Ldc_i4:
                case InstructionCode.Ldarg:
                case InstructionCode.Call:
                    {
                        var arg = BinaryConvert.GetInt32(ref pointer, program);
                        return new IlObject(address, 0, arg);
                    }
                case InstructionCode.Br:
                case InstructionCode.Brfalse:
                case InstructionCode.Brtrue:
                    {
                        var arg = BinaryConvert.GetInt32(ref pointer, program);
                        _labelTargets.Add(new IlAddress(arg));
                        return new IlObject(address, 0, arg);
                    }
                case InstructionCode.Ret:
                case InstructionCode.Add:
                case InstructionCode.Ceq:
                case InstructionCode.Pop:
                case InstructionCode.Nop:
                    {
                        return null;
                    }
                default:
                    {
                        Throw.NotSupportedInstruction(pointer, program[pointer - 1]);
                        return null;
                    }
            }
        }
    }
}
