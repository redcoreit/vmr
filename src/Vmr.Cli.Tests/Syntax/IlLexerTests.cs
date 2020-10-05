using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.Syntax;
using Vmr.Cli.Tests.Internal.Helpers;
using Xunit;

namespace Vmr.Cli.Tests.Internal.Syntax
{
    public class IlLexerTests
    {
        [Theory]
        [MemberData(nameof(ReadTestFiles))]
        public void Lex_sample_files_and_regenerate(string filename, string contnet)
        {
            // Arrange
            var lexer = new IlLexer(contnet);

            // Act
            var tokens = lexer.LexAll().ToList();
            var actual = ReconstructText(tokens);

            // Assert
            Assert.Equal(contnet, actual);
        }

        public static IEnumerable<object[]> ReadTestFiles()
        {
            foreach (var resource in EmbeddedResourceReader.Of<EmbeddedResourceReader>().GetAllFileContent("Vmr.Cli.Tests.Internal.Resources.IlLexerTests", "vril"))
            {
                if (string.IsNullOrEmpty(resource.Content))
                    continue;

                var span = resource.FileReference.Split(".").AsSpan(^2);
                var filename = $"{span[0]}.{span[1]}";

                var content = resource.Content.TrimEnd(new char[] { '\r', '\n' });
                content = $"{content}{Environment.NewLine}";

                yield return new object[] { filename, content };
            }
        }

        private static string ReconstructText(IEnumerable<SyntaxToken> tokens)
        {
            var builder = new StringBuilder();

            foreach (var current in tokens)
            {
                if(current.Kind == SyntaxKind.EndOfFileToken)
                {
                    continue;
                }

                if(builder.Length != 0)
                {
                    if (SyntaxFacts.IsInstruction(current.Kind) || current.Kind == SyntaxKind.LabelToken)
                    {
                        builder.AppendLine();
                    }
                    else
                    {
                        builder.Append(" ");
                    }
                }
                
                var text = SyntaxFacts.GetText(current.Kind);

                if(text is null)
                {
                    text = current.Text;
                }

                builder.Append(text);
            }

            builder.AppendLine();

            return builder.ToString();
        }
    }
}
