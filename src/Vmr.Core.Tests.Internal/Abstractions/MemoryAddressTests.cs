using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Core.Abstractions;
using Xunit;

namespace Vmr.Core.Tests.Internal.Abstractions
{
    public class MemoryAddressTests
    {
        [Fact]
        public void Equality_check_of_zero_and_default_address()
        {
            // Arrange
            MemoryAddress a1 = default;
            MemoryAddress a2 = new(0);

            // Act

            // Assert
            Assert.NotEqual(a1, a2);

        }
    }
}
