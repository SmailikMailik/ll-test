using System;
using LL.Game.Data.Persistence.Documents;
using LL.Game.Data.Sources;
using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using LL.Tests.EditMode.Infrastructure.Saving.Storage;
using NUnit.Framework;

namespace LL.Tests.EditMode.Game.Data
{
    internal sealed class SerializedGameDataSourceTests
    {
        private const int UnsupportedVersion = 2;

        [Test]
        public void UnsupportedVersionIsRejected()
        {
            var saveService = new SaveService(new JsonSaveSerializer(), new MemorySaveStorage());
            var document = CreateEmptyDocument(UnsupportedVersion);
            Assert.That(saveService.TrySave(SerializedGameDataSource.DataKey, document), Is.True);
            var source = new SerializedGameDataSource(saveService);

            Assert.Throws<NotSupportedException>(() => source.Read());
        }

        private static GameDataDocument CreateEmptyDocument(int version) => new
        (
            version,
            Array.Empty<RankDocumentEntry>(),
            Array.Empty<CardDocumentEntry>(),
            Array.Empty<HeroDocumentEntry>(),
            Array.Empty<QuestDocumentEntry>(),
            Array.Empty<RankUpDocumentEntry>(),
            Array.Empty<RewardDocumentEntry>()
        );
    }
}