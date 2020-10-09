using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Syntax;
using Vmr.Instructions;

namespace Vmr.Cli.Helpers
{
    internal class CodeFormatter
    {
        private readonly StringBuilder _builder;

        private CodeFormatter()
        {
            _builder = new StringBuilder();
        }

        public static string Format(DasmProgram dasmProgram)
        {
            var instance = new CodeFormatter();
            instance.Format(dasmProgram.IlObjects, dasmProgram.LabelTargetIlRefs);

            return instance._builder.ToString();
        }

        private void Format(IReadOnlyList<IlObject> ilObjects, IReadOnlyCollection<int> labelTargetIlRefs)
        {
            for (int i = 0; i < ilObjects.Count;)
            {
                var current = ilObjects[i];

                if (labelTargetIlRefs.Contains(current.IlRef.Value))
                {
                    _builder.AppendLine();
                }

                var instruction = (InstructionCode)current.Obj;
                
                FormatInstruction(ref i, current, instruction);
                FormatArgs(ref i, ilObjects, instruction);
            }
        }

        private void FormatInstruction(ref int i, IlObject current, InstructionCode instruction)
        {
            var instructionText = GetInstructionText(instruction);

            var part = $"{current.IlRef.ToString()}:{"  "}{instructionText}";
            _builder.Append(part);

            i++;
        }

        private void FormatArgs(ref int i, IReadOnlyList<IlObject> ilObjects, InstructionCode instruction)
        {
            var start = i;
            var argCount = InstructionFacts.GetArgumentsCount(instruction);

            while (i < start + argCount)
            {
                var arg = ilObjects[i];
                var argText = InstructionFacts.IsBranchingInstruction(instruction)
                    ? GetTargetIlRefText(arg)
                    : GetArgumentText(arg);

                _builder.Append(" ");
                _builder.Append(argText);

                i++;
            }

            _builder.AppendLine();
        }

        private static string GetInstructionText(InstructionCode instruction)
        {
            if (!Enum.TryParse<SyntaxKind>($"OpCode_{instruction}", out var kind))
            {
                throw new CliException($"Instruction '{instruction}' has no syntax kind.");
            }

            var instructionText = SyntaxFacts.GetInstructionText(kind);
            return instructionText;
        }

        private static string GetArgumentText(IlObject arg)
        {
            var text = arg.Obj switch
            {
                int value => value.ToString(CultureInfo.InvariantCulture),
                decimal value => value.ToString(CultureInfo.InvariantCulture),
                string value => $"\"{value}\"",
                null => throw new ArgumentNullException(nameof(arg.Obj)),
                _ => throw new ArgumentOutOfRangeException(nameof(arg.Obj), arg.Obj, null)
            };

            return text;
        }

        private static string GetTargetIlRefText(IlObject arg)
        {
            if (arg.Obj is not int num)
            {
                throw new ArgumentException($"Argument '{arg}' expected to be target il ref.");
            }

            var targetIlRef = new IlRef(num);
            return targetIlRef.ToString();
        }
    }
}
