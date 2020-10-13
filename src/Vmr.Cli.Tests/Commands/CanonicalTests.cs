using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.Commands;
using Vmr.Cli.Options;
using Vmr.Cli.Tests.Internal.Helpers;
using Xunit;

namespace Vmr.Cli.Tests.Internal.Commands
{
    public class CanonicalTests
    {
        private static readonly CanonicalTestSource _testSource;

        static CanonicalTests()
        {
            _testSource = new();
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void Format_command_tests(string name)
        {
            // Arrange
            var content = _testSource.VrilFiles[name];
            var options = new FormatOptions() { FilePath = name };
            var reader = MockFactory.NewFileReader(content);
            var writer = MockFactory.NewFileWriter();
            var configuration = MockFactory.NewConfiguration();

            // Act
            FormatCommand.Run(options, reader, writer, configuration);

            // Assert
            var file = Assert.Single(writer.Files.Values);
            Assert.Equal(content, file);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void Asm_command_tests(string name)
        {
            // Arrange
            var content = _testSource.VrilFiles[name];
            var expected = _testSource.BinFiles[name];
            var options = new AssembleOptions() { FilePath = name };
            var reader = MockFactory.NewFileReader(content);
            var writer = MockFactory.NewFileWriter();
            var configuration = MockFactory.NewConfiguration();

            // Act
            AssembleCommand.Run(options, reader, writer, configuration);

            // Assert
            var file = Assert.Single(writer.Files.Values);
            var actual = Assert.IsType<byte[]>(file);
            Assert.True(Enumerable.SequenceEqual(expected, actual));
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void Dasm_command_tests(string name)
        {
            // Arrange
            var content = _testSource.BinFiles[name];
            var expected = _testSource.BinVrilFiles[name];
            var options = new DisassembleOptions() { FilePath = name };
            var reader = MockFactory.NewFileReader(content);
            var writer = MockFactory.NewFileWriter();
            var configuration = MockFactory.NewConfiguration();

            // Act
            DisassembleCommand.Run(options, reader, writer, configuration);

            // Assert
            var file = Assert.Single(writer.Files.Values);
            var actual = Assert.IsType<string>(file);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void Run_command_tests(string name)
        {
            // Arrange
            var content = _testSource.BinFiles[name];
            var expected = _testSource.TxtFiles[name];
            var options = new RunOptions() { FilePath = name };
            var reader = MockFactory.NewFileReader(content);
            var writer = MockFactory.NewOutputWriter();
            var configuration = MockFactory.NewConfiguration();

            // Act
            RunCommand.Run(options, reader, writer, configuration);

            // Assert
            Assert.Equal(expected, writer.Output);
        }

        public static IEnumerable<object[]> GetTestData()
            => _testSource.Names.Select(m => new[] { m });

        private sealed class CanonicalTestSource
        {
            public CanonicalTestSource()
            {
                var vrilFiles = EmbeddedResourceReader.Of<CanonicalTestSource>().GetAllFileStringContent($"Vmr.Cli.Tests.Internal.Resources.{nameof(CanonicalTests)}", "vril");
                VrilFiles = vrilFiles.ToDictionary(m => TrimFileExt(m.FileReference), m => EnsureLastNewLine(m.Content));

                var binFiles = EmbeddedResourceReader.Of<CanonicalTestSource>().GetAllFileBinaryContent($"Vmr.Cli.Tests.Internal.Resources.{nameof(CanonicalTests)}", "bin");
                BinFiles = binFiles.ToDictionary(m => TrimFileExt(m.FileReference), m => m.Content);

                var binVrilFiles = EmbeddedResourceReader.Of<CanonicalTestSource>().GetAllFileStringContent($"Vmr.Cli.Tests.Internal.Resources.{nameof(CanonicalTests)}.dasm", "dasm");
                BinVrilFiles = binVrilFiles.ToDictionary(m => TrimFileExt(m.FileReference), m => EnsureLastNewLine(m.Content));

                var txtFiles = EmbeddedResourceReader.Of<CanonicalTestSource>().GetAllFileStringContent($"Vmr.Cli.Tests.Internal.Resources.{nameof(CanonicalTests)}.results", "txt");
                TxtFiles = txtFiles.ToDictionary(m => TrimFileExt(m.FileReference), m => EnsureLastNewLine(m.Content));
            }

            public IEnumerable<string> Names => VrilFiles.Keys;

            public IReadOnlyDictionary<string, string> VrilFiles { get; }

            public IReadOnlyDictionary<string, byte[]> BinFiles { get; }

            public IReadOnlyDictionary<string, string> BinVrilFiles { get; }
            
            public IReadOnlyDictionary<string, string> TxtFiles { get; }

            private static string TrimFileExt(string fileReference)
            {
                var nameAndExt = fileReference.Split(".").AsSpan(^2);
                return nameAndExt[0];
            }

            private static string EnsureLastNewLine(string content)
            {
                var result = content.TrimEnd(new char[] { '\r', '\n' });
                result = $"{result}{Environment.NewLine}";

                return result;
            }
        }
    }
}
