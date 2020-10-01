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
        public void Ldc_load_number_from_arg()
        {
            var instructions = new object[] { (int)InstructionCode.Ldc, 0x01 };
            ExecNoDataSingleResultTest(instructions, 0x01);
        }

        [Fact]
        public void Ldc_load_string_from_arg()
        {
            var instructions = new object[] { (int)InstructionCode.Ldc, "test" };
            ExecNoDataSingleResultTest(instructions, "test");
        }

        [Fact]
        public void Ldc_load_string_with_spaces_from_arg()
        {
            var instructions = new object[] { (int)InstructionCode.Ldc, "test test" };
            ExecNoDataSingleResultTest(instructions, "test test");
        }

        [Fact]
        public void Add_load_two_numbers_then_sum()
        {
            var instructions = new object[]
            {
                (int)InstructionCode.Ldc, 0x01,
                (int)InstructionCode.Ldc, 0x02,
                (int)InstructionCode.Add
            };

            ExecNoDataSingleResultTest(instructions, 3);
        }

        [Fact]
        public void Pop_remove_data_from_stack()
        {
            // Assert
            var instructions = new object[] { (int)InstructionCode.Ldc, 0x01, (int)InstructionCode.Pop };
            var data = Enumerable.Empty<object>();
            var vm = new VirtualMachine(instructions);

            // Act
            vm.Execute(data);
            var actualResult = vm.GetStack();

            // Assert
            Assert.Equal(0, actualResult.Count);
        }

        private void ExecNoDataSingleResultTest<TResult>(IReadOnlyList<object> instructions, TResult expected)
        {
            // Assert
            var data = Enumerable.Empty<object>();
            var vm = new VirtualMachine(instructions);

            // Act
            vm.Execute(data);
            var actualResult = vm.GetStack();

            // Assert
            Assert.Equal(1, actualResult.Count);
            var result = Assert.Single(actualResult);
            var number = Assert.IsType<TResult>(result);
            Assert.Equal(expected, number);
        }
    }
}
