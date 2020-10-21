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
            builder.Method("main", isEntryPoint: true);
            builder.Ldc_i4(0x01);

            // Act
            var actual = builder.GetIlProgram().IlMethods.Single().IlObjects.Select(m => m.Obj).ToHashSet();

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
            builder.Method("main", isEntryPoint: true);
            builder.Ldstr("test test");

            // Act
            var actual = builder.GetIlProgram().IlMethods.Single().IlObjects.Select(m => m.Obj).ToHashSet();

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
            builder.Method("main", isEntryPoint: true);
            builder.Ldc_i4(0x01);
            builder.Ldc_i4(2);
            builder.Add();

            // Act
            var actual = builder.GetIlProgram().IlMethods.Single().IlObjects.Select(m => m.Obj).ToHashSet();

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
            builder.Method("main", isEntryPoint: true);
            builder.Ldc_i4(0x01);
            builder.Ldc_i4(2);
            builder.Ceq();

            // Act
            var actual = builder.GetIlProgram().IlMethods.Single().IlObjects.Select(m => m.Obj).ToHashSet();

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
                4u,
                InstructionCode.Add,
                InstructionCode.Ret,
            };
            var builder = GetBuilder();

            // Act
            _ = builder.GetIlProgram(); // Test reproducable property of CodeBuilder

            var program = builder.GetIlProgram();
            var ilObjects = program.IlMethods.SelectMany(m => m.IlObjects).ToArray();
            var actual = ilObjects.Select(m => m.Obj).ToList();

            // Assert
            Assert.Equal(2, program.IlMethods.Count);
            Assert.Equal(4u, program.IlMethods[0].Address.Value);
            Assert.Equal(10u, program.IlMethods[1].Address.Value);
            Assert.Equal(10u, program.EntryPoint.Value);
            Assert.Equal(4u, ilObjects[0].Address.Value);
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
