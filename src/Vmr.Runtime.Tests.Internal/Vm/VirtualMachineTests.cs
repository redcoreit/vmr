using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Instructions;
using Vmr.Runtime.Vm;
using Xunit;

namespace Vmr.Runtime.Tests.Internal.Vm
{
    public class VirtualMachineTests
    {
        [Fact]
        public void Ldc_i4_load_number_from_arg()
        {
            var builder = new CodeBuilder();
            builder.Ldc(1);
            var instructions = builder.Assemble();

            ExecNoDataSingleResultTest(instructions, 0x01);
        }

        [Fact]
        public void Ldstr_load_string_from_arg()
        {
            var builder = new CodeBuilder();
            builder.Ldstr("test");
            var instructions = builder.Assemble();

            ExecNoDataSingleResultTest(instructions, "test");
        }

        [Fact]
        public void Ldcstr_load_string_with_spaces_from_arg()
        {
            var builder = new CodeBuilder();
            builder.Ldstr("test test");
            var instructions = builder.Assemble();

            ExecNoDataSingleResultTest(instructions, "test test");
        }

        [Fact]
        public void Add_load_two_numbers_then_sum()
        {
            var builder = new CodeBuilder();
            builder.Ldc(1);
            builder.Ldc(2);
            builder.Add();
            var instructions = builder.Assemble();

            ExecNoDataSingleResultTest(instructions, 3);
        }

        [Fact]
        public void Pop_remove_data_from_stack()
        {
            // Assert
            var builder = new CodeBuilder();
            builder.Ldc(1);
            builder.Pop();
            var instructions = builder.Assemble();

            var vm = new VirtualMachine();

            // Act
            vm.Execute(instructions);
            var actualResult = vm.GetStack();

            // Assert
            Assert.Empty(actualResult);
        }

        private void ExecNoDataSingleResultTest<TResult>(byte[] instructions, TResult expected)
        {
            // Assert
            var vm = new VirtualMachine();

            // Act
            vm.Execute(instructions);
            var actualResult = vm.GetStack();

            // Assert
            Assert.NotEmpty(actualResult);
            var result = Assert.Single(actualResult);
            var number = Assert.IsType<TResult>(result);
            Assert.Equal(expected, number);
        }
    }
}
