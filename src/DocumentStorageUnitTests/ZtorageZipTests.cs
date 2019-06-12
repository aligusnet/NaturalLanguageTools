using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

using DocumentStorage;

namespace DocumentStorageUnitTests
{
    public class ZtorageZipTests
    {
        [Fact]
        public void ZipReadWriteTest()
        {
            var docs1 = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0, 0), "Title 1"),
                    "Title 1. This is the first document"
                ),
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0, 1), "Title 2"),
                    "Title 2. This is the second document"
                ),
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0, 2), "Title 3"),
                    "Title 3. This is thethirs document"
                ),
            };

            var docs2 = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentMetadata(new DocumentId(1, 0), "Title 4"),
                    "Title 4. This is the first document from the second collection"
                ),
            };

            var storage = new List<DocumentCollection<string>>
            {
                DocumentCollection<string>.Make(0, docs1),
                DocumentCollection<string>.Make(1, docs2),
            };

            var path = @"C:\path";

            var fileSystem = new MockFileSystem();
            fileSystem.Directory.CreateDirectory(path);

            var writer = new StorageZipWriter<string>(path, new StringDocumentDataSerializer(), fileSystem);
            var reader = new StorageZipReader<string>(path, new StringDocumentDataSerializer(), fileSystem);

            writer.Write(storage);

            var deserializedStorage = reader.Read().ToList();

            Assert.Equal(storage.Count, deserializedStorage.Count);
            Assert.Equal(storage[0].Documents[1].Metadata.Id, deserializedStorage[0].Documents[1].Metadata.Id);
            Assert.Equal(storage[0].Documents[1].Data, deserializedStorage[0].Documents[1].Data);
        }
    }
}
