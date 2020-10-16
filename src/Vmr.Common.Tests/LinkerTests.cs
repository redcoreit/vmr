using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common;
using Vmr.Common.Assemble;
using Vmr.Common.Instructions;
using Xunit;

namespace Vmr.Common.Tests
{
    public class LinkerTests
    {
        [Fact]
        public void Link_label_no_instruction()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Label("start");

            // Act
            var instructions = builder.GetIlProgram().IlMethods.Single().IlObjects.Select(m => m.Obj).ToArray();

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

            // Act
            var instructions = builder.GetIlProgram().IlMethods.Single().IlObjects.Select(m => m.Obj).ToArray();

            // Assert
            Assert.Equal(2, instructions.Length);
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

            // Act
            var instructions = builder.GetIlProgram().IlMethods.Single().IlObjects.Select(m => m.Obj).ToArray();

            // Assert
            Assert.Equal(3, instructions.Length);
            Assert.Equal(InstructionCode.Br, instructions[0]);
            Assert.Equal(sizeof(InstructionCode) + sizeof(int), instructions[1]);
            Assert.Equal(InstructionCode.Nop, instructions[2]);
        }
    }
}
