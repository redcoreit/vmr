using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Vmr.Assembler.Exceptions;
using Vmr.Core.Abstractions;

namespace Vmr.Assembler
{
    // TODO (RH -): Simple something that works. Will be replaced by proper lexer.
    public sealed class SimpleHumanReadableFileProcessor
    {
        private SimpleHumanReadableFileProcessor()
        {
        }

        public static IReadOnlyList<object> Process(FileInfo vrilFile)
        {
            using var stream = vrilFile.OpenRead();
            return Process(stream);
        }

        public static IReadOnlyList<object> Process(Stream stream)
        {
            var instance = new SimpleHumanReadableFileProcessor();
            var result = instance.LoadFile(stream);

            return result;
        }

        private IReadOnlyList<object> LoadFile(Stream stream)
        {
            var result = new List<object>();
            StreamReader sr = new StreamReader(stream, Encoding.UTF8);

            string? line;
            int lineIndex = 0;

            while ((line = sr.ReadLine()) is not null)
            {
                var instructions = LoadLine(lineIndex, line);
                result.AddRange(instructions);
                lineIndex++;
            }

            return result;
        }

        private IEnumerable<object> LoadLine(int lineIndex, string line)
        {
            var words = line.Split(' ', '\t', StringSplitOptions.RemoveEmptyEntries);

            var result = line.Contains("\"")
                ? LoadStringInstructionLine()
                : LoadNonStringInstructionLine();

            return result;

            IEnumerable<object> LoadStringInstructionLine()
            {
                var instruction = (string?)words[0];

                if (instruction is null)
                {
                    throw new InvalidOperationException(nameof(instruction)); // this is not expected
                }

                if (instruction.StartsWith(";;"))
                {
                    yield break; // comment begins, we stop processing the rest of the line
                }

                yield return LoadInstruction(instruction);
                yield return LoadArgument(string.Join(' ', words.Skip(1)));
            }

            IEnumerable<object> LoadNonStringInstructionLine()
            {
                for (var i = 0; i < words.Length; i++)
                {
                    var word = (string?)words[i];

                    if (word is null)
                    {
                        throw new InvalidOperationException(nameof(word)); // this is not expected
                    }

                    if (word.Equals(";;"))
                    {
                        yield break; // comment begins, we stop processing the rest of the line
                    }

                    yield return i == 0
                        ? LoadInstruction(word)
                        : LoadArgument(word);
                }
            }
        }

        private int LoadInstruction(string word)
        {
            if (!Enum.TryParse<InstructionCode>(word, out var instructionCode))
            {
                throw new AssemblerException("First code of the line must be an instruction.");
            }

            return (int)instructionCode;
        }

        private object LoadArgument(string word)
        {
            var span = word.AsSpan();

            if (span[0] == '"')
            {
                return word.Trim('"');
            }
            else if(word.StartsWith("0x") && int.TryParse(span.Slice(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var number))
            {
                return number;
            }
            else if (char.IsDigit(span[0]) && int.TryParse(word, NumberStyles.Integer, CultureInfo.InvariantCulture, out number))
            {
                return number;
            }

            throw new AssemblerException($"Not supported argument: '{word}'"); 
        }
    }
}
