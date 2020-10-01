using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Vmr.Instructions.Tests.Instructions
{
    public class AssemblerTests
    {
        [Fact]
        public void Ldc_load_number_from_arg()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Ldc("");

            // Act
            var actual = builder.Assemble();

            // Assert
            Assert.Equal(3, actual.Count);
            Assert.Equal((byte)InstructionCode.Ldc, actual[0]);
            Assert.Equal(InstructionFacts.StringInitializer, actual[1]);
            Assert.Equal(InstructionFacts.StringTerminator, actual[2]);
        }

        [Fact]
        public void Ldc_load_string_with_spaces_from_arg()
        {
            // Arrange
            var expectedTextBytes = Encoding.UTF8.GetBytes("test test");
            var builder = new CodeBuilder();
            builder.Ldc("test test");

            // Act
            var actual = builder.Assemble();

            // Assert
            Assert.Equal(3 + expectedTextBytes.Length, actual.Count);
            var actualTextBytes = actual.ToArray().AsSpan(2..^1).ToArray();

            Assert.Equal((byte)InstructionCode.Ldc, actual[0]);
            Assert.Equal(InstructionFacts.StringInitializer, actual[1]);
            Assert.True(Enumerable.SequenceEqual(expectedTextBytes, actualTextBytes));
            Assert.Equal(InstructionFacts.StringTerminator, actual.Last());
        }

        [Fact]
        public void Add_load_two_numbers_then_sum()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Ldc(0x01);
            builder.Ldc(2);
            builder.Add();

            // Act
            var actual = builder.Assemble();

            // Assert
            Assert.Equal(11, actual.Count);
            Assert.Equal((byte)InstructionCode.Ldc, actual[0]);
            
            // 0x01
            Assert.Equal((byte)0, actual[1]);
            Assert.Equal((byte)0, actual[2]);
            Assert.Equal((byte)0, actual[3]);
            Assert.Equal((byte)1, actual[4]);

            Assert.Equal((byte)InstructionCode.Ldc, actual[5]);

            // 2
            Assert.Equal((byte)0, actual[6]);
            Assert.Equal((byte)0, actual[7]);
            Assert.Equal((byte)0, actual[8]);
            Assert.Equal((byte)2, actual[9]);

            Assert.Equal((byte)InstructionCode.Add, actual[10]);
        }
    }
}
