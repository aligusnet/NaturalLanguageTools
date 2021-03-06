﻿using System.Collections.Generic;
using System.Linq;

using Moq;
using Xunit;

using Corpus;

namespace InformationRetrieval.Test
{
    using Tokens = IEnumerable<string>;
    using HashedBlock = Block<int[]>;

    public class DocumentHasherTests
    {
        
        [Fact]
        public void HashDocumentTest()
        {
            var corpus = CreateCorpus();

            var hasher = new DocumentHasher();

            var hashed = hasher.Transform(corpus).First().Documents;

            Assert.Equal(7, hashed[0].Data.Length);
            Assert.Equal(7, hashed[1].Data.Length);
            Assert.NotEqual(hashed[0].Data, hashed[1].Data);

            hashed[0].Data[1] = hashed[1].Data[1];
            hashed[0].Data[5] = hashed[1].Data[5];
            Assert.Equal(hashed[0].Data, hashed[1].Data);
        }

        [Fact]
        public void HasherIsConsistentTest()
        {
            var corpus = CreateCorpus();

            var hashed1 = new DocumentHasher().Transform(corpus).First().Documents;
            var hashed2 = new DocumentHasher().Transform(corpus).First().Documents;

            Assert.Equal(hashed1[0].Data, hashed2[0].Data);
            Assert.Equal(hashed1[1].Data, hashed2[1].Data);
        }

        [Fact]
        public void HashDocumentCorpusTest()
        {
            var corpus = CreateCorpus();

            var hashed = new List<IEnumerable<Block<int[]>>>();

            var reader = new Mock<ICorpusReader<Tokens>>();
            reader.Setup(r => r.Read()).Returns(corpus);

            var writer = new Mock<ICorpusWriter<int[]>>();
            writer.Setup(w => w.Write(It.IsAny<IEnumerable<HashedBlock>>()))
                  .Callback((IEnumerable<HashedBlock> d) => hashed.Add(d));

            var tokenizer = new DocumentHasher();
            tokenizer.Transform(reader.Object, writer.Object);

            Assert.Single(hashed);
            var hashedBlock = hashed[0].First();
            Assert.Equal(2, hashedBlock.Documents.Count);
        }

        private static IEnumerable<Block<Tokens>> CreateCorpus()
        {
            var docs = new List<Document<Tokens>>
            {
                new Document<Tokens> (
                    new DocumentMetadata(new DocumentId(0), "Title 1"),
                    "Title 1 This is the first document".Split()
                ),
                new Document<Tokens> (
                    new DocumentMetadata(new DocumentId(1), "Title 2"),
                    "Title 2 This is the second document".Split()
                ),
            };

            return new List<Block<Tokens>>
            {
                Block<Tokens>.Make(0, docs),
            };
        }
    }
}
