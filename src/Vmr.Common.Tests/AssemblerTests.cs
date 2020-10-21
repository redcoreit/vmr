﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common;
using Vmr.Common.Instructions;
using Xunit;

namespace Vmr.Common.Tests
{
    public class AssemblerTests
    {
        [Fact]
        public void Ldc_load_number_from_arg()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Ldstr("");

            // Act
            var actual = builder.GetBinaryProgram();

            // Assert
            Assert.Equal(6, actual.Length);
            Assert.Equal((byte)InstructionCode.Ldstr, actual[4]);
            Assert.Equal(InstructionFacts.Eos, actual[5]);
        }

        [Fact]
        public void Ldstr_load_string_with_spaces_from_arg()
        {
            // Arrange
            var expectedTextBytes = Encoding.UTF8.GetBytes("test test");
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Ldstr("test test");

            // Act
            var actual = builder.GetBinaryProgram();

            // Assert
            Assert.Equal(4 + 1 + expectedTextBytes.Length + 1, actual.Length);
            Assert.Equal((byte)InstructionCode.Ldstr, actual[4]);

            // test test
            var actualTextBytes = actual.ToArray().AsSpan(5..^1).ToArray();
            Assert.True(expectedTextBytes.SequenceEqual(actualTextBytes));
            Assert.Equal(InstructionFacts.Eos, actual.Last());
        }

        [Fact]
        public void Add_load_two_numbers_then_sum()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Ldc_i4(0x01);
            builder.Ldc_i4(2);
            builder.Add();

            // Act
            var actual = builder.GetBinaryProgram();

            // Assert
            Assert.Equal(15, actual.Length);

            // header: entry point address
            Assert.Equal((byte)4, actual[0]);
            Assert.Equal((byte)0, actual[1]);
            Assert.Equal((byte)0, actual[2]);
            Assert.Equal((byte)0, actual[3]);

            // Ldc.i4
            Assert.Equal((byte)InstructionCode.Ldc_i4, actual[4]);

            // 0x01
            Assert.Equal((byte)1, actual[5]);
            Assert.Equal((byte)0, actual[6]);
            Assert.Equal((byte)0, actual[7]);
            Assert.Equal((byte)0, actual[8]);

            // Ldc.i4
            Assert.Equal((byte)InstructionCode.Ldc_i4, actual[9]);

            // 2
            Assert.Equal((byte)2, actual[10]);
            Assert.Equal((byte)0, actual[11]);
            Assert.Equal((byte)0, actual[12]);
            Assert.Equal((byte)0, actual[13]);

            // Add
            Assert.Equal((byte)InstructionCode.Add, actual[14]);
        }

        [Fact]
        public void Method_call_load_two_numbers_then_add()
        {
            // Arrange
            var builder = GetBuilder();
            var pointer = 0;

            // Act
            _ = builder.GetIlProgram(); // Test reproducable property of CodeBuilder
            var program = builder.GetBinaryProgram();

            // Assert

            // Entry Point
            Assert.Equal(0, pointer);
            Assert.Equal(10u, BinaryConvert.GetUInt32(ref pointer, program));

            // two
            var method_two = pointer;
            Assert.Equal(4, pointer);
            Assert.Equal(InstructionCode.Ldc_i4, BinaryConvert.GetInstructionCode(ref pointer, program));
            Assert.Equal(2, BinaryConvert.GetInt32(ref pointer, program));
            Assert.Equal(InstructionCode.Ret, BinaryConvert.GetInstructionCode(ref pointer, program));

            // main
            Assert.Equal(10, pointer);
            Assert.Equal(InstructionCode.Ldc_i4, BinaryConvert.GetInstructionCode(ref pointer, program));
            Assert.Equal(1, BinaryConvert.GetInt32(ref pointer, program));
            Assert.Equal(InstructionCode.Call, BinaryConvert.GetInstructionCode(ref pointer, program));
            Assert.Equal(method_two, BinaryConvert.GetInt32(ref pointer, program));
            Assert.Equal(InstructionCode.Add, BinaryConvert.GetInstructionCode(ref pointer, program));
            Assert.Equal(InstructionCode.Ret, BinaryConvert.GetInstructionCode(ref pointer, program));

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
