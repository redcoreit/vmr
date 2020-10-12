﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Transactions;
using Vmr.Common;
using Vmr.Common.Exeptions;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Common.Disassemble
{
    public class Disassembler
    {
        private readonly List<IlObject> _ilObjects;
        private readonly HashSet<int> _labelTargetIlRefs;

        private int _pointer;

        private Disassembler()
        {
            _pointer = 0;
            _ilObjects = new List<IlObject>();
            _labelTargetIlRefs = new HashSet<int>();
        }

        public static IlProgram GetProgram(byte[] program)
        {
            var instance = new Disassembler();
            var span = program.AsSpan();
            return instance.GetProgram(span);
        }

        private IlProgram GetProgram(ReadOnlySpan<byte> program)
        {
            while (_pointer < program.Length)
            {
                var instruction = GetInstruction(program[_pointer]);

                _ilObjects.Add(new IlObject(_pointer, instruction));
                _pointer++;

                ReadArguments(instruction, program);
            }

            return new IlProgram(_ilObjects, _labelTargetIlRefs);
        }

        private InstructionCode GetInstruction(byte current)
        {
            try
            {
                return BinaryConvert.GetInstructionCode(current);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex.Message);
                throw new VmrException($"Not supported instruction. IlRef: {new IlRef(_pointer).ToString()} Code: {InstructionFacts.Format(current)}");
            }
        }

        private void ReadArguments(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            switch (instruction)
            {
                case InstructionCode.Ldstr:
                    {
                        var arg = BinaryConvert.GetString(ref _pointer, program);
                        _ilObjects.Add(new IlObject(_pointer, arg));
                        _pointer++;
                        break;
                    }
                case InstructionCode.Ldloc:
                case InstructionCode.Stloc:
                case InstructionCode.Ldc_i4:
                    {
                        var arg = BinaryConvert.GetInt32(ref _pointer, program);
                        _ilObjects.Add(new IlObject(_pointer, arg));
                        _pointer++;
                        break;
                    }
                case InstructionCode.Br:
                case InstructionCode.Brfalse:
                case InstructionCode.Brtrue:
                    {
                        var arg = BinaryConvert.GetInt32(ref _pointer, program);
                        _ilObjects.Add(new IlObject(_pointer, arg));
                        _labelTargetIlRefs.Add(arg);
                        _pointer++;
                        break;
                    }
                case InstructionCode.Add:
                case InstructionCode.Ceq:
                case InstructionCode.Pop:
                case InstructionCode.Nop:
                    {
                        break;
                    }
                default:
                    throw new VmrException($"Not supported instruction. IlRef: {new IlRef(_pointer).ToString()} Code: {InstructionFacts.Format(instruction)}");
            }
        }
    }
}