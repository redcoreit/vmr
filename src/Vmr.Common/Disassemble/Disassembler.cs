using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Transactions;
using Vmr.Common;
using Vmr.Common.Exeptions;
using Vmr.Common.Instructions;
using Vmr.Common.Linking;
using Vmr.Common.Primitives;

namespace Vmr.Common.Disassemble
{
    public class Disassembler
    {
        private readonly List<IlObject> _ilObjects;
        private readonly HashSet<IlAddress> _methodTargets;
        private readonly HashSet<IlAddress> _labelTargets;

        private int _segment;
        private int _ilRef;

        private Disassembler()
        {
            _segment = 0;
            _ilRef = 0;
            _ilObjects = new List<IlObject>();
            _methodTargets = new HashSet<IlAddress>();
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
            IlAddress? entryPoint = null;
            var entryPointValue = BinaryConvert.GetInt32(ref _ilRef, program);

            while (_ilRef < program.Length)
            {
                var instruction = GetInstruction(program[_ilRef]);
                HandleSegment(instruction);

                var address = GetIlAddress();

                if (address.Value == entryPointValue)
                {
                    entryPoint = address;
                }

                _ilObjects.Add(new IlObject(address, 0, instruction));
                _ilRef++;

                ReadArguments(instruction, program);
            }

            if (entryPoint is null)
            {
                throw new VmrException($"Entrypoint not found.");
            }

            return new IlProgram(entryPoint, Array.Empty<IlMethod>(), _labelTargets);

            void HandleSegment(InstructionCode instruction)
            {
                if (instruction != InstructionCode.Ret)
                {
                    return;
                }

                _segment = _ilRef;
                _ilRef = 0;
            }
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
                throw new VmrException($"Not supported instruction. IlRef: {_ilRef.ToString()} Code: {InstructionFacts.Format(current)}");
            }
        }

        private void ReadArguments(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            var address = GetIlAddress();

            switch (instruction)
            {
                case InstructionCode.Ldstr:
                    {
                        var arg = BinaryConvert.GetString(ref _ilRef, program);
                        _ilObjects.Add(new IlObject(address, 0,arg));
                        _ilRef++;
                        break;
                    }
                case InstructionCode.Ldloc:
                case InstructionCode.Stloc:
                case InstructionCode.Ldc_i4:
                case InstructionCode.Call:
                    {
                        var arg = BinaryConvert.GetInt32(ref _ilRef, program);
                        _ilObjects.Add(new IlObject(address, 0, arg));
                        _ilRef++;
                        break;
                    }
                case InstructionCode.Br:
                case InstructionCode.Brfalse:
                case InstructionCode.Brtrue:
                    {
                        var arg = BinaryConvert.GetInt32(ref _ilRef, program);
                        _ilObjects.Add(new IlObject(address, 0, arg));
                        _labelTargets.Add(new IlAddress(arg)); // Cross segment jumps are not supported
                        _ilRef++;
                        break;
                    }
                case InstructionCode.Ret:
                case InstructionCode.Add:
                case InstructionCode.Ceq:
                case InstructionCode.Pop:
                case InstructionCode.Nop:
                    {
                        break;
                    }
                default:
                    throw new VmrException($"Not supported instruction. IlRef: {_ilRef.ToString()} Code: {InstructionFacts.Format(instruction)}");
            }
        }

        private IlAddress GetIlAddress()
            => new IlAddress(_ilRef);
    }
}
