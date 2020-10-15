using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common;
using Vmr.Common.Instructions;
using Xunit;

namespace Vmr.Common.Tests
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
            var actual = builder.GetIlProgram().IlObjects.Select(m => m.Obj).ToHashSet();

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
            var actual = builder.GetIlProgram().IlObjects.Select(m => m.Obj).ToHashSet();

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
            var actual = builder.GetIlProgram().IlObjects.Select(m => m.Obj).ToHashSet();

            // Assert
            Assert.Subset(expected, actual);
            Assert.Superset(expected, actual);
        }

        [Fact]
        public void Ceq_load_two_numbers_then_compare()
        {
            // Arrange
            var expected = new object[] { InstructionCode.Ldc_i4, 0x01, InstructionCode.Ldc_i4, 2, InstructionCode.Ceq }.ToHashSet();
            var builder = new CodeBuilder();
            builder.Ldc_i4(0x01);
            builder.Ldc_i4(2);
            builder.Ceq();

            // Act
            var actual = builder.GetIlProgram().IlObjects.Select(m => m.Obj).ToHashSet();

            // Assert
            Assert.Subset(expected, actual);
            Assert.Superset(expected, actual);
        }

        [Fact]
        public void Method_call_load_two_numbers_then_add()
        {
            // Arrange
            var expected = new object[] 
            {
                // unused method removed
                InstructionCode.Ldc_i4,
                2,
                InstructionCode.Ret,
                InstructionCode.Ldc_i4,
                1,
                InstructionCode.Call,
                0, 
                InstructionCode.Add,
                InstructionCode.Ret,
            };
            var builder = GetBuilder();

            // Act
            var ilObjects = builder.GetIlProgram().IlObjects;
            var actual = ilObjects.Select(m => m.Obj);

            // Assert
            Assert.True(Enumerable.SequenceEqual(expected, actual));

            static CodeBuilder GetBuilder()
            {
                var builder = new CodeBuilder();

                //one
                builder.Method("one", 0, false);
                builder.Ldc_i4(1);
                builder.Ret();

                //two
                builder.Method("two", 0, false);
                builder.Ldc_i4(2);
                builder.Ret();

                //main
                builder.Method("main", 0, true);
                builder.Ldc_i4(1);
                builder.Call("two");
                builder.Add();
                builder.Ret();

                return builder;
            }
        }
    }
}
