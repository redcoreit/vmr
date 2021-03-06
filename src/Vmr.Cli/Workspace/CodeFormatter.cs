﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.Exceptions;
using Vmr.Cli.Helpers;
using Vmr.Cli.Workspace.Syntax;
using Vmr.Common;
using Vmr.Common.Instructions;
using Vmr.Common.Linking;
using Vmr.Common.ObjectModel;

namespace Vmr.Cli.Workspace
{
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
            for (var idx = 0; idx < program.IlMethods.Count; idx++)
            {
                if (idx != 0)
                {
                    _builder.AppendLine();
                }

                var method = (IlMethod)program.IlMethods[idx];
                var methodName = program.MethodNames[method.Address];

                _builder.Append(SyntaxFacts.GetAttributeText(SyntaxKind.Attribute_Method));
                _builder.Append(' ');
                _builder.Append(methodName);
                _builder.AppendLine(":");

                if (method.Address == program.EntryPoint)
                {
                    _builder.Append(' ', formatSettings.LineIndentSize);
                    _builder.AppendLine(SyntaxFacts.GetAttributeText(SyntaxKind.Attribute_Entrypoint));
                }

                if(method.Args != 0)
                {
                    _builder.Append(' ', formatSettings.LineIndentSize);
                    _builder.Append(SyntaxFacts.GetAttributeText(SyntaxKind.Attribute_Args));
                    _builder.Append(' ');
                    _builder.AppendLine(method.Args.ToString());
                }

                var locals = GetLocalsSize(method.IlObjects);

                if(locals != 0)
                {
                    _builder.Append(' ', formatSettings.LineIndentSize);
                    _builder.Append(SyntaxFacts.GetAttributeText(SyntaxKind.Attribute_Locals));
                    _builder.Append(' ');
                    _builder.AppendLine(locals.ToString());
                }

                FormatMethod(method, program, formatSettings);
            }

            static int GetLocalsSize(IReadOnlyList<IlObject> ilObjects)
            {
                int locals = -1;
                var isLdlocOrStloc = false;

                foreach (var current in ilObjects)
                {
                    if (isLdlocOrStloc)
                    {
                        var value = (int)current.Obj;

                        if (value > locals)
                        {
                            locals = value;
                        }

                        isLdlocOrStloc = false;
                    }

                    if (current.Obj is InstructionCode code)
                    {
                        isLdlocOrStloc = code == InstructionCode.Ldloc || code == InstructionCode.Stloc;
                    }
                }

                return locals + 1;
            }
        }


        private void FormatMethod(IlMethod method, IlProgram program, CodeFormatSettings formatSettings)
        {
            var idx = 0;
            while (idx < method.IlObjects.Count)
            {
                var current = method.IlObjects[idx];
                var instruction = (InstructionCode)current.Obj;

                FormatComment(current.Address, program, formatSettings);
                FormatLabel(method, program, formatSettings, current);
                FormatInstruction(method, current, instruction, formatSettings);
                idx++;

                FormatArgs(ref idx, method, program, instruction, formatSettings);
            }
        }

        private void FormatComment(IlAddress address, IlProgram program, CodeFormatSettings formatSettings)
        {
            if (!program.Comments.TryGetValue(address, out var text))
            {
                return;
            }

            foreach (var line in text)
            {
                _builder.Append(' ', formatSettings.LineIndentSize);
                _builder.Append('/', 2);
                _builder.AppendLine(line);
            }
        }

        private void FormatLabel(IlMethod method, IlProgram program, CodeFormatSettings formatSettings, IlObject current)
        {
            if (program.LabelTargets.Contains(current.Address))
            {
                _builder.AppendLine();

                if (formatSettings.UseIlRefPrefix)
                {
                    return;
                }

                var relativeAddress = current.Address.Value - method.Address.Value;

                if (!program.LabelNames.TryGetValue(current.Address, out var name))
                {
                    name = relativeAddress.ToIlRef();
                }

                _builder.Append(name);
                _builder.Append(':');
                _builder.AppendLine();
            }
        }

        private void FormatInstruction(IlMethod method, IlObject current, InstructionCode instruction, CodeFormatSettings formatSettings)
        {
            var instructionText = GetInstructionText(instruction);
            var relativeAddress = current.Address.Value - method.Address.Value;

            if (formatSettings.UseIlRefPrefix)
            {
                _builder.Append(' ', formatSettings.LineIndentSize);
                _builder.Append(relativeAddress.ToIlRef());
                _builder.Append(':');
            }

            _builder.Append(' ', formatSettings.InstructionIndentSize);
            _builder.Append(instructionText);

            relativeAddress += InstructionFacts.SizeOfOpCode;
        }

        private static string GetInstructionText(InstructionCode instruction)
        {
            if (!Enum.TryParse<SyntaxKind>($"OpCode_{instruction}", out var kind))
                throw new CliException($"Instruction '{instruction}' has no syntax kind.");

            var instructionText = SyntaxFacts.GetInstructionText(kind);
            return instructionText;
        }

        private void FormatArgs(ref int idx, IlMethod method, IlProgram program, InstructionCode instruction, CodeFormatSettings formatSettings)
        {
            var start = idx;
            var argCount = InstructionFacts.GetArgumentsCount(instruction);

            while (idx < start + argCount)
            {
                var arg = method.IlObjects[idx];
                var ilRef = arg.Address.Value - method.Address.Value;
                var argText = InstructionFacts.IsBranchingInstruction(instruction)
                    ? GetTargetIlRefText(method, program.LabelNames, arg)
                    : instruction == InstructionCode.Call
                    ? program.MethodNames[(int)arg.Obj]
                    : GetArgumentText(arg);

                var argSize = GetArgumentSize(arg);

                _builder.Append(" ");
                _builder.Append(argText);

                idx++;
            }

            _builder.AppendLine();
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

        private static string GetTargetIlRefText(IlMethod method, IReadOnlyDictionary<IlAddress, string> labelNames, IlObject arg)
        {
            if (arg.Obj is not int num)
                throw new ArgumentException($"Argument '{arg}' expected to be target il ref.");

            var address = new IlAddress(num);

            if (!labelNames.TryGetValue(address, out var name))
            {
                var relativeAddress = address.Value - method.Address.Value;
                name = relativeAddress.ToIlRef();
            }

            return name;
        }

        private static int GetArgumentSize(IlObject arg)
        {
            var size = arg.Obj switch
            {
                int value => sizeof(int),
                decimal value => sizeof(decimal),
                string value => BinaryConvert.GetBytes(value).Length,
                null => throw new ArgumentNullException(nameof(arg.Obj)),
                _ => throw new ArgumentOutOfRangeException(nameof(arg.Obj), arg.Obj, null)
            };

            return size;
        }
    }
}
