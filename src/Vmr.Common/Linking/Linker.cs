using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Vmr.Common.Assemble;
using Vmr.Common.Exeptions;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Common.Linking
{
    public sealed class Linker
    {
        private readonly LabelTableBuilder _labelTableBuilder;

        private IlAddress? _entryPoint;

        private Linker()
        {
            _labelTableBuilder = new LabelTableBuilder();
        }

        internal static IlProgram Run(Method entryPoint, IReadOnlyList<Method> methods)
        {
            if (!methods.Any())
                throw new VmrException($"Empty program cannot be linked.");

            var instance = new Linker();
            var result = instance.RunInternal(entryPoint, methods);
            return result;
        }

        private IlProgram RunInternal(Method entryPoint, IReadOnlyList<Method> methods)
        {
            var callTree = CallTree.Create(entryPoint, methods);
            var ilMethods = LinkMethods(InstructionFacts.SizeProgramHeader, callTree);
            var labelTable = _labelTableBuilder.Build();

            if (_entryPoint is null)
            {
                throw new VmrException("Entry point not found by linker.");
            }

            return new IlProgram(_entryPoint, ilMethods, labelTable.GetTargets(), labelTable.GetLabelNames());
        }

        private IReadOnlyList<IlMethod> LinkMethods(int startAddress, CallTree callTree)
        {
            var result = new List<IlMethod>();
            var methods = callTree.Flatten().Select(m => m.Method).OrderBy(m => m.Order).ToList();
            var methodAddressLookup = GetMethodAddressLookup(startAddress, methods);

            foreach (var method in methods)
            {
                var address = methodAddressLookup[method.Name];
                var ilObjects = LinkNodes(methodAddressLookup, address, method.Nodes);
                var ilMethod = new IlMethod(new IlAddress(address), method.Size, ilObjects);

                if (method.IsEntryPoint)
                {
                    _entryPoint = ilMethod.Address;
                }

                result.Add(ilMethod);
            }

            return result;

            static IReadOnlyDictionary<string, int> GetMethodAddressLookup(int startAddress, IReadOnlyList<Method> methods)
            {
                var result = new Dictionary<string, int>();

                var methodAddress = startAddress;
                foreach (var method in methods)
                {
                    result.Add(method.Name, methodAddress);
                    methodAddress += method.Size;
                }

                return result;
            }
        }

        private IReadOnlyList<IlObject> LinkNodes(IReadOnlyDictionary<string, int> methodAddressLookup, int startAddress, IReadOnlyList<ProgramNode> nodes)
        {
            var result = new List<IlObject>();
            var branchAddressLookup = GetInstructionAddressLookup(startAddress, nodes);
            var address = startAddress;
            ArgRewriteType? rewriteType = null;

            foreach (var item in branchAddressLookup)
            {
                _labelTableBuilder.AddTarget(item.Key, new IlAddress(item.Value));
            }

            foreach (var node in nodes)
            {
                object? valueOverride = null;

                if (rewriteType is object)
                {
                    var name = (string)((Argument)node).Value;

                    valueOverride = rewriteType == ArgRewriteType.CallTarget
                        ? (object?)methodAddressLookup[name]
                        : rewriteType == ArgRewriteType.BranchTarget
                        ? (object?)branchAddressLookup[name]
                        : throw new InvalidOperationException(nameof(rewriteType))
                        ;

                    rewriteType = null;
                }

                if (node is Instruction inst)
                {
                    rewriteType = inst.InstructionCode == InstructionCode.Call
                        ? (ArgRewriteType?)ArgRewriteType.CallTarget
                        : InstructionFacts.IsBranchingInstruction(inst.InstructionCode)
                        ? (ArgRewriteType?)ArgRewriteType.BranchTarget
                        : null
                        ;
                }

                var ilObject = node switch
                {
                    LabelDeclaration m => null,
                    Instruction m => CreateInstruction(m, address),
                    Argument m => CreateArgument(m, address, valueOverride),
                    _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
                };

                if (ilObject is null)
                {
                    continue;
                }

                result.Add(ilObject);
                address += node.Size;
            }

            return result;

            static IReadOnlyDictionary<string, int> GetInstructionAddressLookup(int startAddress, IReadOnlyList<ProgramNode> nodes)
            {
                var result = new Dictionary<string, int>();

                var address = startAddress;
                foreach (var node in nodes)
                {
                    if (node is LabelDeclaration label)
                    {
                        result.Add(label.Name, address);
                    }
                    else
                    {
                        address += node.Size;
                    }
                }

                return result;
            }

            static IlObject CreateInstruction(Instruction instruction, int address)
                => new IlObject(new IlAddress(address), instruction.Size, instruction.InstructionCode);

            static IlObject CreateArgument(Argument argument, int address, object? valueOverride = null)
                => new IlObject(new IlAddress(address), argument.Size, valueOverride ?? argument.Value);
        }

        enum ArgRewriteType
        {
            CallTarget = 1,
            BranchTarget,
        }
    }
}
