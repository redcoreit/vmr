using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Common.Linking
{
    internal sealed class CallTree
    {
        public CallTree(CallTreeNode root)
            => Root = root;

        public CallTreeNode Root { get; }

        public static CallTree Create(Method entryPoint, IReadOnlyList<Method> methods)
        {
            var methodLookup = methods.ToDictionary(m => m.Name);
            var handled = new HashSet<Method>();

            var root = Recursive(methodLookup, handled, entryPoint);
            return new CallTree(root);
        }

        public IReadOnlyList<CallTreeNode> Flatten()
        {
            var result = new Stack<CallTreeNode>();
            var stack = new Stack<CallTreeNode>();
            stack.Push(Root);

            while (stack.Count != 0)
            {
                var current = stack.Pop();

                foreach (var item in current.References)
                {
                    stack.Push(item);
                }

                result.Push(current);
            }

            return result.ToArray();
        }

        private static CallTreeNode Recursive(IReadOnlyDictionary<string, Method> methodLookup, HashSet<Method> handled, Method currentMethod)
        {
            var methodReferences = GetMethodReferences(currentMethod);
            var nodes = new List<CallTreeNode>();

            foreach (var methodName in methodReferences)
            {
                var method = methodLookup[methodName];
                CallTreeNode node;

                if (handled.Contains(method))
                {
                    node = new CallTreeNode(currentMethod);
                }
                else
                {
                    node = Recursive(methodLookup, handled, method);
                }

                nodes.Add(node);
            }

            return new CallTreeNode(currentMethod, nodes);
        }

        private static IReadOnlyCollection<string> GetMethodReferences(Method currentMethod)
        {
            var calls = new HashSet<string>();

            for (int idx = 0; idx < currentMethod.Nodes.Count; idx++)
            {
                var currentNode = currentMethod.Nodes[idx];

                if (currentNode is Instruction inst && inst.InstructionCode == InstructionCode.Call)
                {
                    var arg = (Argument)currentMethod.Nodes[++idx];
                    var methodName = (string)arg.Value;
                    calls.Add(methodName);
                }
            }

            return calls;
        }
    }
}
