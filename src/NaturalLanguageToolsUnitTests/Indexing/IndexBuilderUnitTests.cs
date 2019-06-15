﻿using System.Collections.Generic;

using Moq;
using Xunit;

using DocumentStorage;
using NaturalLanguageTools.Indexing;

namespace NaturalLanguageToolsUnitTests.Indexing
{
    public class IndexBuilderUnitTests
    {
        [Fact]
        public void IndexBuilderTest()
        {
            var docs1 = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0, 0), "Title 1"),
                    "abc"
                ),
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0, 1), "Title 2"),
                    "defa"
                ),
            };

            var docs2 = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentMetadata(new DocumentId(1, 0), "Title 3"),
                    "bd"
                ),
            };

            var storage = new List<DocumentCollection<string>>
            {
                DocumentCollection<string>.Make(0, docs1),
                DocumentCollection<string>.Make(1, docs2),
            };

            var indexedWords = new List<(DocumentId, char, int)>();

            var index = new Mock<IBuildableIndex<char>>();
            index.Setup(x => x.IndexWord(It.IsAny<DocumentId>(), It.IsAny<char>(), It.IsAny<int>()))
                 .Callback<DocumentId, char, int>((id, word, pos) => indexedWords.Add((id, word, pos)));

            var indexer = new IndexBuilder<char, string>(index.Object);
            indexer.IndexStorage(storage);

            var expectedIndexedWords = new List<(DocumentId, char, int)>
            {
                (new DocumentId(0, 0), 'a', 0), (new DocumentId(0, 0), 'b', 1), (new DocumentId(0, 0), 'c', 2),
                (new DocumentId(0, 1), 'd', 0), (new DocumentId(0, 1), 'e', 1), (new DocumentId(0, 1), 'f', 2), (new DocumentId(0, 1), 'a', 3),
                (new DocumentId(1, 0), 'b', 0), (new DocumentId(1, 0), 'd', 1),
            };

            Assert.Equal(expectedIndexedWords, indexedWords);
        }
    }
}
