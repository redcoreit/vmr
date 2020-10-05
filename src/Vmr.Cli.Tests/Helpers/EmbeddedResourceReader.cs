using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vmr.Cli.Tests.Internal.Helpers
{
    public class EmbeddedResourceReader
    {
        private readonly Assembly _assembly;

        private EmbeddedResourceReader(Type assemblyType)
        {
            _assembly = assemblyType.Assembly;
        }

        public static EmbeddedResourceReader Of(object reference)
            => new EmbeddedResourceReader(reference.GetType());

        public static EmbeddedResourceReader Of<TType>()
            => new EmbeddedResourceReader(typeof(TType));

        public string GetFileText(string folderFqdn, string filename)
            => GetFileText(string.Concat(folderFqdn, ".", filename));

        public Stream GetFileStream(string folderFqdn, string filename)
            => GetFileStream(string.Concat(folderFqdn, ".", filename));

        public string GetFileText(string fileFqdn)
        {
            using var stream = GetFileStream(fileFqdn);

            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();

        }

        public Stream GetFileStream(string fileFqdn)
        {
            var stream = _assembly.GetManifestResourceStream(fileFqdn);

            if (stream == null)
                throw new Exception($"Resource not found! {fileFqdn}");

            return stream;
        }

        internal IEnumerable<(string FileReference, string Content)> GetAllFileContent(string directoryFqdn, string? fileExtensionFilter = null)
        {
            var fileDirectory = $"{directoryFqdn}.";

            foreach (var resurce in _assembly.GetManifestResourceNames())
            {
                if (!resurce.StartsWith(fileDirectory))
                    continue;

                if (fileExtensionFilter is not null && !resurce.EndsWith($".{fileExtensionFilter}"))
                    continue;

                yield return (resurce, GetFileText(resurce));
            }
        }
    }
}
