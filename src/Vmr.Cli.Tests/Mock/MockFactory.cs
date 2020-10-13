using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Cli.IO;
using Vmr.Cli.Tests.Internal.Mock;

namespace Vmr.Cli.Tests.Internal.Helpers
{
    internal class MockFactory
    {
        internal static IFileReader NewFileReader(string content)
            => new InterceptorFileReader(content);

        internal static IFileReader NewFileReader(byte[] content)
            => new InterceptorFileReader(content);

        internal static InterceptorFileWriter NewFileWriter()
            => new InterceptorFileWriter();

        internal static InterceptiorOutputWriter NewOutputWriter()
            => new InterceptiorOutputWriter();

        internal static IConfiguration NewConfiguration(params (string Key, string Value)[] values)
            => new MockConfiguration(values.ToDictionary(m => m.Key, m => m.Value));
    }
}
