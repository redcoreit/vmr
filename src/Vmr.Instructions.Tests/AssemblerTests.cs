using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Vmr.Instructions.Tests
{
    public class AssemblerTests
    {
        [Fact]
        public void Ldc_load_number_from_arg()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Ldstr("");

            // Act
            var actual = builder.Compile();

            // Assert
            Assert.Equal(2, actual.Length);
            Assert.Equal((byte)InstructionCode.Ldstr, actual[0]);
            Assert.Equal(InstructionFacts.StringTerminator, actual[1]);
        }

        [Fact]
        public void Ldstr_load_string_with_spaces_from_arg()
        {
            // Arrange
            var expectedTextBytes = Encoding.UTF8.GetBytes("test test");
            var builder = new CodeBuilder();
            builder.Ldstr("test test");

            // Act
            var actual = builder.Compile();

            // Assert
            Assert.Equal(2 + expectedTextBytes.Length, actual.Length);
            Assert.Equal((byte)InstructionCode.Ldstr, actual[0]);

            // test test
            var actualTextBytes = actual.ToArray().AsSpan(1..^1).ToArray();
            Assert.True(expectedTextBytes.SequenceEqual(actualTextBytes));
            Assert.Equal(InstructionFacts.StringTerminator, actual.Last());
        }

        [Fact]
        public void Add_load_two_numbers_then_sum()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Ldc_i4(0x01);
            builder.Ldc_i4(2);
            builder.Add();

            // Act
            var actual = builder.Compile();

            // Assert
            Assert.Equal(11, actual.Length);
            Assert.Equal((byte)InstructionCode.Ldc_i4, actual[0]);

            // 0x01
            Assert.Equal((byte)0, actual[1]);
            Assert.Equal((byte)0, actual[2]);
            Assert.Equal((byte)0, actual[3]);
            Assert.Equal((byte)1, actual[4]);

            Assert.Equal((byte)InstructionCode.Ldc_i4, actual[5]);

            // 2
            Assert.Equal((byte)0, actual[6]);
            Assert.Equal((byte)0, actual[7]);
            Assert.Equal((byte)0, actual[8]);
            Assert.Equal((byte)2, actual[9]);

            Assert.Equal((byte)InstructionCode.Add, actual[10]);
        }
    }
}
