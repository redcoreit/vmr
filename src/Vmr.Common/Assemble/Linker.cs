using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vmr.Common.Exeptions;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Common.Assemble
{
    public static class Linker
    {
        internal static void Run(List<IlObject> program, MethodTable methodTable, LabelTable lableTable)
        {
            RemoveUnusedMethods(program, methodTable);
            LinkMethods(program, methodTable);
            LinkLabels(program, methodTable, lableTable);


        }

        private static void RemoveUnusedMethods(List<IlObject> program, MethodTable methodTable)
        {
            var originalProgram = program.ToList();
            program.Clear();

            var methodNames = methodTable.GetNames();
            var methods = methodTable.GetTargets();

            var segmentShift = 0;
            IlAddress segment = default;

            for (var idx = 0; idx < originalProgram.Count; idx++)
            {
                var ilObj = originalProgram[idx];

                if (!methods.Contains(ilObj.Address))
                {
                    RewriteAdd(ilObj);
                    continue;
                }

                if(methodTable.Entrypoint == ilObj.Address)
                {
                    RewriteAdd(ilObj);
                    continue;
                }

                segment = ilObj.Address.Segment;
                var name = methodNames[segment];

                if (methodTable.HasReference(name))
                {
                    RewriteAdd(ilObj);
                    continue;
                }

                SeekToNextSegment(methods, originalProgram, idx, out var size);

                segmentShift += size;
                idx += size;
            }

            static void SeekToNextSegment(IReadOnlyCollection<IlAddress> methods, IReadOnlyList<IlObject> program, int start, out int size)
            {
                size = 0;

                foreach (var current in program.Skip(start + 1))
                {
                    if(methods.Contains(current.Address))
                    {
                        break;
                    }

                    size++;
                }
            }

            void RewriteAdd(IlObject ilObj)
            {
                var rewriten = ilObj;

                if (segmentShift != 0)
                {
                    var address = new IlAddress(segment.Value - segmentShift, ilObj.Address.IlRef);
                    rewriten = new IlObject(address, ilObj.Obj);
                }

                program.Add(rewriten);
            }
        }

        private static void LinkMethods(List<IlObject> program, MethodTable methodTable)
        {
            var methods = methodTable.GetTargets();
            IlAddress segment = default;

            for (var idx = 0; idx < program.Count; idx++)
            {
                var ilObj = program[idx];

                if (methods.Contains(ilObj.Address))
                {
                    segment = ilObj.Address.Segment;
                }

                if (methodTable.TryGetReference(ilObj.Address, out var method))
                {
                    var targetIlAddress = methodTable.GetTarget(method);
                    program[idx] = new IlObject(ilObj.Address, targetIlAddress.Value);
                }
            }
        }

        private static void LinkLabels(List<IlObject> program, MethodTable methodTable, LabelTable lableTable)
        {
            var methodNames = methodTable.GetNames();
            var methods = methodTable.GetTargets();
            IlAddress segment = default;

            for (var idx = 0; idx < program.Count; idx++)
            {
                var ilObj = program[idx];

                if (methods.Contains(ilObj.Address))
                {
                    segment = ilObj.Address.Segment;
                }

                if (lableTable.TryGetReference(ilObj.Address, out var label))
                {
                    var targetIlRef = lableTable.GetTarget(label);

                    if (targetIlRef.Segment != segment)
                    {
                        var name = methodNames[targetIlRef.Segment];
                        throw new VmrException($"Cross segment jump detected in method '{name}' IlRef '{ilObj.Address.IlRef}'.");
                    }

                    var targetIlAddress = new IlAddress(segment.Value, targetIlRef.IlRef);

                    program[idx] = new IlObject(ilObj.Address, targetIlAddress.Value);
                }
            }
        }
    }
}
