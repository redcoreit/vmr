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
using Vmr.Common.Primitives;

namespace Vmr.Common.Disassemble
{
    public class Disassembler
    {
        private readonly List<IlObject> _ilObjects;
        private readonly HashSet<IlAddress> _methodTargets;
        private readonly HashSet<IlAddress> _labelTargets;

        private int _pointer;
        private IlAddress? _entryPoint;

        private Disassembler()
        {
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
            var entryPointValue = BinaryConvert.GetInt32(ref _pointer, program);
            _entryPoint = new IlAddress(entryPointValue);

            _methodTargets.Add(_entryPoint);

            while (_pointer < program.Length)
            {
                var address = _pointer;
                var instruction = GetInstruction(program);
                _ilObjects.Add(new IlObject(address, 0, instruction));

                ReadArguments(instruction, program);
            }

            if (_entryPoint is null)
            {
                throw new VmrException($"Entrypoint not found.");
            }

            var ilMethods = ReconstructMethods(_ilObjects, program.Length);

            return new IlProgram(_entryPoint, ilMethods, _labelTargets);
        }

        private InstructionCode GetInstruction(ReadOnlySpan<byte> program)
        {
            try
            {
                return BinaryConvert.GetInstructionCode(ref _pointer, program);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex.Message);
                Throw.NotSupportedInstruction(_pointer, program[_pointer - 1]);
                throw;
            }
        }

        private void ReadArguments(InstructionCode instruction, ReadOnlySpan<byte> program)
        {
            var address = new IlAddress(_pointer);

            switch (instruction)
            {
                case InstructionCode.Ldstr:
                    {
                        var arg = BinaryConvert.GetString(ref _pointer, program);
                        _ilObjects.Add(new IlObject(address, 0, arg));
                        break;
                    }
                case InstructionCode.Ldloc:
                case InstructionCode.Stloc:
                case InstructionCode.Ldc_i4:
                    {
                        var arg = BinaryConvert.GetInt32(ref _pointer, program);
                        _ilObjects.Add(new IlObject(address, 0, arg));
                        break;
                    }
                case InstructionCode.Call:
                    {
                        var arg = BinaryConvert.GetInt32(ref _pointer, program);
                        _ilObjects.Add(new IlObject(address, 0, arg));
                        _methodTargets.Add(new IlAddress(arg));
                        break;
                    }
                case InstructionCode.Br:
                case InstructionCode.Brfalse:
                case InstructionCode.Brtrue:
                    {
                        var arg = BinaryConvert.GetInt32(ref _pointer, program);
                        _ilObjects.Add(new IlObject(address, 0, arg));
                        _labelTargets.Add(new IlAddress(arg));
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
                    {
                        Throw.NotSupportedInstruction(_pointer, program[_pointer - 1]);
                        break;
                    }
            }
        }

        private IReadOnlyList<IlMethod> ReconstructMethods(IReadOnlyList<IlObject> _ilObjects, int programEnd)
        {
            var methodAddresses = _methodTargets.OrderBy(m => m.Value).ToArray().AsReadOnlySpan();
            var orderedIlObjects = _ilObjects.OrderBy(m => m.Address).ToArray().AsReadOnlySpan();

            var enumerator = orderedIlObjects.GetEnumerator();
            var result = new List<IlMethod>();

            if (!enumerator.MoveNext())
            {
                throw new VmrException("Empty program detected.");
            }

            for (int idx = 0; idx < methodAddresses.Length; idx++)
            {
                var current = methodAddresses[idx].Value;
                var next = idx + 1 < methodAddresses.Length
                    ? methodAddresses[idx + 1].Value
                    : programEnd;


                var ilObjects = new List<IlObject>();

                while (enumerator.Current.Address.Value < next)
                {
                    ilObjects.Add(enumerator.Current);

                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                }

                result.Add(new IlMethod(current, 0, ilObjects));
            }

            return result;
        }
    }
}
