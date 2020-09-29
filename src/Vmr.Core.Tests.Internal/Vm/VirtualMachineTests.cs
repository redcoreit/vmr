using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Core.Abstractions;
using Xunit;

namespace Vmr.Core.Tests.Internal.Vm
{
    public class VirtualMachineTests
    {
        [Fact]
        public void Ldc_load_number_from_arg()
        {
            var instructions = new object[] { (int)InstructionCode.Ldc, 0x01 };
            ExecSingleResultTest(instructions, 0x01);
        }

        [Fact]
        public void Ldc_load_string_from_arg()
        {
            var instructions = new object[] { (int)InstructionCode.Ldc, "test" };
            ExecSingleResultTest(instructions, "test");
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

            ExecSingleResultTest(instructions, 3);
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

        private void ExecSingleResultTest<TResult>(IReadOnlyList<object> instructions, TResult expected)
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
