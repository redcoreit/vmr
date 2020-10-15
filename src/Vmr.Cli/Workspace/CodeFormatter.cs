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
            instance.FormatInternal(program, formatSettings ?? DefaultFormatSettings);

            return instance._builder.ToString();
        }

        private void FormatInternal(IlProgram program, CodeFormatSettings formatSettings)
        {
            var idx = 0;
            while (idx < program.IlObjects.Count)
            {
                var current = program.IlObjects[idx];
                var instruction = (InstructionCode)current.Obj;
                
                FormatLabel(program, formatSettings, current);
                FormatInstruction(ref idx, current, instruction, formatSettings);
                FormatArgs(ref idx, program, instruction, formatSettings);
            }
        }

        private void FormatLabel(IlProgram program, CodeFormatSettings formatSettings, IlObject current)
        {
            if (program.LabelTargets.Contains(current.Address))
            {
                _builder.AppendLine();

                if (formatSettings.UseIlRefPrefix)
                {
                    return;
                }

                if (!program.LabelNames.TryGetValue(current.Address, out var name))
                {
                    name = current.Address.IlRef.ToString();
                }

                
                _builder.Append(name);
                _builder.Append(':');
                _builder.AppendLine();
            }
        }

        private void FormatInstruction(ref int idx, IlObject current, InstructionCode instruction, CodeFormatSettings formatSettings)
        {
            var instructionText = GetInstructionText(instruction);

            if (formatSettings.UseIlRefPrefix)
            {
                _builder.Append(current.Address.IlRef.ToString());
                _builder.Append(':');
            }

            _builder.Append(' ', formatSettings.IndentSize);
            _builder.Append(instructionText);

            idx++;
        }

        private void FormatArgs(ref int idx, IlProgram program, InstructionCode instruction, CodeFormatSettings formatSettings)
        {
            var start = idx;
            var argCount = InstructionFacts.GetArgumentsCount(instruction);

            while (idx < start + argCount)
            {
                var arg = program.IlObjects[idx];
                var argText = InstructionFacts.IsBranchingInstruction(instruction)
                    ? GetTargetIlRefText(program, arg)
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

        private static string GetTargetIlRefText(IlProgram program, IlObject arg)
        {
            if (arg.Obj is not int num)
                throw new ArgumentException($"Argument '{arg}' expected to be target il ref.");

            var address = new IlAddress(arg.Address.Segment.Value, num);

            if (!program.LabelNames.TryGetValue(address, out var name))
            {
                var targetIlRef = address.IlRef;
                name = targetIlRef.ToString();
            }

            return name;
        }
    }
}
