using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Vmr.Assembler.Tests
{
    public class SimpleHumanReadableFileProcessorTests
    {
        [Fact]
        public void Ldc_load_number_from_arg()
        {
            // Arrange
            var expected = new object[] { InstructionCode.Ldc, 0x01 }.ToHashSet();
            var fileContent =
                "Ldc 0x01"
                ;

            // Act
            var stack = Exec(fileContent);

            // Assert
            AssertStack(stack, 1);
        }
        [Fact]
        public void Ldc_load_string_from_arg()
        {
            // Arrange
            var fileContent =
                "Ldc \"test\""
                ;

            // Act
            var stack = Exec(fileContent);

            // Assert
            AssertStack(stack, "test");
        }
        [Fact]
        public void Ldc_load_string_with_spaces_from_arg()
        {
            // Arrange
            var fileContent =
                "Ldc \"test test\""
                ;

            // Act
            var stack = Exec(fileContent);

            // Assert
            AssertStack(stack, "test test");
        }
        [Fact]
        public void Add_load_two_numbers_then_sum()
        {
            // Arrange
            var fileContent =
                "Ldc 0x01\r\n" +
                "Ldc 2\r\n" +
                "Add"
                ;

            // Act
            var stack = Exec(fileContent);

            // Assert
            AssertStack(stack, 3);
        }
        [Fact]
        public void Pop_remove_data_from_stack()
        {
            // Arrange
            var fileContent =
                "Ldc 0x01\n" +
                "Pop"
                ;

            // Act
            var stack = Exec(fileContent);

            // Assert
            AssertStack(stack);
        }

        private static Stack<object> Exec(string fileContent)
        {
            var bytes = Encoding.UTF8.GetBytes(fileContent);
            using var ms = new MemoryStream(bytes);
            var code = SimpleHumanReadableFileProcessor.Process(ms);
            var vm = new VirtualMachine(code);

            vm.Execute(Enumerable.Empty<object>());

            return vm.GetStack();
        }

        private static void AssertStack(Stack<object> stack, params object[] expectedItems)
        {
            Assert.Equal(expectedItems.Length, stack.Count);

            var actualItems = stack.Reverse().ToArray();

            for (var i = 0; i < expectedItems.Length; i++)
            {
                var expected = (object?)expectedItems[i];
                var actual = (object?)actualItems[i];

                if(expected is null)
                {
                    throw new NotSupportedException();
                }

                if (actual is null)
                {
                    throw new NotSupportedException();
                }

                Assert.Equal(expected, actual);
            }
        }
    }
}
