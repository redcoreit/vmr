using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Vmr.Common.Exeptions;
using Vmr.Common.Instructions;
using Vmr.Common.ObjectModel;
using Vmr.Common.Primitives;

namespace Vmr.Common.Linking
{
    public sealed class Linker
    {
        private readonly LinkTableBuilder _labelTableBuilder;
        private readonly LinkTableBuilder _methodTableBuilder;
        private readonly Dictionary<IlAddress, List<string>> _comments; // TODO (RH codereview): Find a better solution

        private IlAddress? _entryPoint;

        private Linker()
        {
            _labelTableBuilder = new LinkTableBuilder();
            _methodTableBuilder = new LinkTableBuilder();
            _comments = new Dictionary<IlAddress, List<string>>();
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
            var methodTable = _methodTableBuilder.Build();

            if (_entryPoint is null)
            {
                throw new VmrException("Entry point not found by linker.");
            }

            return new IlProgram(_entryPoint, ilMethods, labelTable.GetTargets(), labelTable.GetNames(), methodTable.GetNames(), _comments);
        }

        private IReadOnlyList<IlMethod> LinkMethods(int startAddress, CallTree callTree)
        {
            var result = new List<IlMethod>();
            var methods = callTree.Flatten().Select(m => m.Method).OrderBy(m => m.Order).ToList();
            GetMethodAddressLookup(_methodTableBuilder, startAddress, methods);

            foreach (var method in methods)
            {
                var address = _methodTableBuilder.Targets[method.Name];
                var ilObjects = LinkNodes(address.Value, method.Nodes);
                var ilMethod = new IlMethod(address, method.Size, ilObjects);

                if (method.IsEntryPoint)
                {
                    _entryPoint = ilMethod.Address;
                }

                result.Add(ilMethod);
            }

            return result;

            static void GetMethodAddressLookup(LinkTableBuilder methodTableBuilder, int startAddress, IReadOnlyList<Method> methods)
            {
                var methodAddress = startAddress;
                foreach (var method in methods)
                {
                    methodTableBuilder.AddTarget(method.Name, methodAddress);
                    methodAddress += method.Size;
                }
            }
        }

        private IReadOnlyList<IlObject> LinkNodes(int startAddress, IReadOnlyList<ProgramNode> nodes)
        {
            GetInstructionAddressLookup(_labelTableBuilder, startAddress, nodes);

            var result = new List<IlObject>();
            var address = startAddress;
            ArgRewriteType? rewriteType = null;

            foreach (var node in nodes)
            {
                object? valueOverride = null;

                if (rewriteType is object)
                {
                    var name = (string)((Argument)node).Value;

                    valueOverride = rewriteType == ArgRewriteType.CallTarget
                        ? (object?)_methodTableBuilder.Targets[name].Value
                        : rewriteType == ArgRewriteType.BranchTarget
                        ? (object?)_labelTableBuilder.Targets[name].Value
                        : throw new InvalidOperationException(nameof(rewriteType))
                        ;

                    if (rewriteType == ArgRewriteType.CallTarget)
                    {
                        _methodTableBuilder.AddReference(address, name);
                    }

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
                    Comment m => HandleComment(m, address),
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

            static void GetInstructionAddressLookup(LinkTableBuilder labelTableBuilder, int startAddress, IReadOnlyList<ProgramNode> nodes)
            {
                var address = startAddress;
                foreach (var node in nodes)
                {
                    if (node is LabelDeclaration label)
                    {
                        labelTableBuilder.AddTarget(label.Name, address);
                    }
                    else
                    {
                        address += node.Size;
                    }
                }
            }

            static IlObject CreateInstruction(Instruction instruction, int address)
                => new IlObject(new IlAddress(address), instruction.Size, instruction.InstructionCode);

            static IlObject CreateArgument(Argument argument, int address, object? valueOverride = null)
                => new IlObject(new IlAddress(address), argument.Size, valueOverride ?? argument.Value);

            IlObject? HandleComment(Comment node, int address)
            {
                var key = new IlAddress(address);

                if (!_comments.ContainsKey(key))
                {
                    _comments[key] = new List<string>();
                }

                _comments[key].Add(node.Text);
                return null;
            }
        }

        enum ArgRewriteType
        {
            CallTarget = 1,
            BranchTarget,
        }
    }
}
