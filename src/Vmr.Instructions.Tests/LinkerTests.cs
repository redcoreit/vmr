﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Vmr.Instructions.Tests
{
    public class LinkerTests
    {
        [Fact]
        public void Link_label_no_instruction()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Label("start");
            var instructions = builder.GetInstructions().ToList();

            // Act
            Linker.LinkLabels(instructions, builder.GetLabelInfo());

            // Assert
            Assert.Empty(instructions);
        }

        [Fact]
        public void Link_label_epsilon_jump()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Br("start");
            builder.Label("start");
            var instructions = builder.GetInstructions().ToList();

            // Act
            Linker.LinkLabels(instructions, builder.GetLabelInfo());

            // Assert
            Assert.Equal(2, instructions.Count);
            Assert.Equal(InstructionCode.Br, instructions[0]);
            Assert.Equal(sizeof(InstructionCode) + sizeof(int), instructions[1]);
        }

        [Fact]
        public void Link_label_with_nop()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Br("start");
            builder.Label("start");
            builder.Nop();
            var instructions = builder.GetInstructions().ToList();

            // Act
            Linker.LinkLabels(instructions, builder.GetLabelInfo());

            // Assert
            Assert.Equal(3, instructions.Count);
            Assert.Equal(InstructionCode.Br, instructions[0]);
            Assert.Equal(sizeof(InstructionCode) + sizeof(int), instructions[1]);
            Assert.Equal(InstructionCode.Nop, instructions[2]);
        }
    }
}