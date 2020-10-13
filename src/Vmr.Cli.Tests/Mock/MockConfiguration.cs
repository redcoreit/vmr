using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Cli.Tests.Internal.Mock
{
    internal sealed class MockConfiguration : IConfiguration
    {
        private readonly Dictionary<string, string> _values;

        public MockConfiguration()
            :this(new Dictionary<string, string>())
        {
        }

        public MockConfiguration(Dictionary<string, string> values)
        {
            _values = values;
        }

        public string this[string key]
        { 
            get => _values[key]; 
            set => _values[key] = value; 
        }

        public IEnumerable<IConfigurationSection> GetChildren() 
            => Enumerable.Empty<IConfigurationSection>();
        
        public IChangeToken GetReloadToken() 
            => throw new NotImplementedException();
        
        public IConfigurationSection GetSection(string key) 
            => throw new NotImplementedException();
    }
}
