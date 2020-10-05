using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Instructions;
using Xunit;

namespace Vmr.Instructions.Tests
{
    public class CodeBuilderTests
    {
        [Fact]
        public void Ldc_load_number_from_arg()
        {
            // Arrange
            var expected = new object[] { InstructionCode.Ldc_i4, 0x01 }.ToHashSet();
            var builder = new CodeBuilder();
            builder.Ldc_i4(0x01);

            // Act
            var actual = builder.GetInstructions().ToHashSet();

            // Assert
            Assert.Subset(expected, actual);
            Assert.Superset(expected, actual);
        }

        [Fact]
        public void Ldc_load_string_with_spaces_from_arg()
        {
            // Arrange
            var expected = new object[] { InstructionCode.Ldstr, "test test" }.ToHashSet();
            var builder = new CodeBuilder();
            builder.Ldstr("test test");

            // Act
            var actual = builder.GetInstructions().ToHashSet();

            // Assert
            Assert.Subset(expected, actual);
            Assert.Superset(expected, actual);
        }

        [Fact]
        public void Add_load_two_numbers_then_sum()
        {
            // Arrange
            var expected = new object[] { InstructionCode.Ldc_i4, 0x01, InstructionCode.Ldc_i4, 2, InstructionCode.Add }.ToHashSet();
            var builder = new CodeBuilder();
            builder.Ldc_i4(0x01);
            builder.Ldc_i4(2);
            builder.Add();

            // Act
            var actual = builder.GetInstructions().ToHashSet();

            // Assert
            Assert.Subset(expected, actual);
            Assert.Superset(expected, actual);
        }
    }
}
