using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common;
using Vmr.Common.Assemble;
using Vmr.Common.Exeptions;
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
            builder.Method("main", isEntryPoint: true);
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
            builder.Method("main", isEntryPoint: true);
            builder.Br("start");
            builder.Label("start");

            // Act
            var instructions = builder.GetIlProgram().IlMethods.Single().IlObjects.Select(m => m.Obj).ToArray();

            // Assert
            Assert.Equal(2, instructions.Length);
            Assert.Equal(InstructionCode.Br, instructions[0]);
            Assert.Equal(10, instructions[1]);
        }

        [Fact]
        public void Link_label_with_nop()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Br("start");
            builder.Label("start");
            builder.Nop();

            // Act
            var instructions = builder.GetIlProgram().IlMethods.Single().IlObjects.Select(m => m.Obj).ToArray();

            // Assert
            Assert.Equal(3, instructions.Length);
            Assert.Equal(InstructionCode.Br, instructions[0]);
            Assert.Equal(10, instructions[1]);
            Assert.Equal(InstructionCode.Nop, instructions[2]);
        }

        [Fact]
        public void Link_label_with_cross_method_branch()
        {
            // Arrange
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Label("start");
            builder.Nop();
            builder.Method("other");
            builder.Br("start");

            // Act

            // Assert
            Assert.Throws<VmrException>(() => builder.GetIlProgram());
        }
    }
}
