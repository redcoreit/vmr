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
using Vmr.Cli.Workspace.Syntax;
using Vmr.Common.Instructions;
using Vmr.Common.Primitives;

namespace Vmr.Cli.Workspace
{
    internal record CodeFormatSettings(bool UseIlRefPrefix = false, int IndentSize = 4);

    internal class CodeFormatter
    {
        private static readonly CodeFormatSettings DefaultFormatSettings;

        private readonly StringBuilder _builder;

        static CodeFormatter()
        {
            DefaultFormatSettings = new CodeFormatSettings();
        }

        private CodeFormatter()
        {
            _builder = new StringBuilder();
        }

        public static string Format(IlProgram program, CodeFormatSettings? formatSettings = null)
        {
            var instance = new CodeFormatter();
            instance.Format(program.IlObjects.ToList(), program.LabelTargetIlRefs, formatSettings ?? DefaultFormatSettings);

            return instance._builder.ToString();
        }

        private void Format(IReadOnlyList<IlObject> ilObjects, IReadOnlyCollection<int> labelTargetIlRefs, CodeFormatSettings formatSettings)
        {
            for (var idx = 0; idx < ilObjects.Count;)
            {
                var current = ilObjects[idx];

                if (labelTargetIlRefs.Contains(current.IlRef.Value))
                    _builder.AppendLine();

                var instruction = (InstructionCode)current.Obj;

                FormatInstruction(ref idx, current, instruction, formatSettings);
                FormatArgs(ref idx, ilObjects, instruction, formatSettings);
            }
        }

        private void FormatInstruction(ref int idx, IlObject current, InstructionCode instruction, CodeFormatSettings formatSettings)
        {
            var instructionText = GetInstructionText(instruction);

            if(formatSettings.UseIlRefPrefix)
            {
                _builder.Append(current.IlRef.ToString());
                _builder.Append(':');
            }

            _builder.Append(' ', formatSettings.IndentSize);
            _builder.Append(instructionText);

            idx++;
        }

        private void FormatArgs(ref int idx, IReadOnlyList<IlObject> ilObjects, InstructionCode instruction, CodeFormatSettings formatSettings)
        {
            var start = idx;
            var argCount = InstructionFacts.GetArgumentsCount(instruction);

            while (idx < start + argCount)
            {
                var arg = ilObjects[idx];
                var argText = InstructionFacts.IsBranchingInstruction(instruction)
                    ? GetTargetIlRefText(arg)
                    : GetArgumentText(arg);

                _builder.Append(" ");
                _builder.Append(argText);

                idx++;
            }

            _builder.AppendLine();
        }

        private static string GetInstructionText(InstructionCode instruction)
        {
            if (!Enum.TryParse<SyntaxKind>($"OpCode_{instruction}", out var kind))
                throw new CliException($"Instruction '{instruction}' has no syntax kind.");

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
                throw new ArgumentException($"Argument '{arg}' expected to be target il ref.");

            var targetIlRef = new IlRef(num);
            return targetIlRef.ToString();
        }
    }
}
