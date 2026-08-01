using System;
using System.Text;
using LL.Game.Data.Persistence.Documents;
using LL.Game.Data.Sources;
using LL.Infrastructure.Saving;
using LL.Infrastructure.Saving.Serialization;
using NUnit.Framework;

namespace LL.Tests.EditMode.Persistence
{
    internal sealed class GameDataPersistenceTests
    {
        [Test]
        public void DocumentRoundTripPreservesPolymorphicRequirements()
        {
            var serializer = new JsonSaveSerializer();
            var document = CreateDocument(GameDataDocument.CurrentVersion);

            Assert.That(serializer.TrySerialize(document, out var bytes), Is.True);

            var json = Encoding.UTF8.GetString(bytes);
            Assert.That(json, Does.Contain("\"Type\": \"quest\""));
            Assert.That(json, Does.Contain("\"Type\": \"payment\""));
            Assert.That(json, Does.Contain("\"RequiredCount\": 20"));
            Assert.That(json, Does.Not.Contain("RequiredAmount"));
            Assert.That(json, Does.Not.Contain("$type"));
            Assert.That(serializer.TryDeserialize(bytes, out GameDataDocument restored), Is.True);
            Assert.That(restored.Version, Is.EqualTo(1));
            Assert.That(
                restored.RankUps[0].Options[0].Requirements[0],
                Is.TypeOf<QuestRankUpRequirementDocumentEntry>());
            Assert.That(
                ((QuestRankUpRequirementDocumentEntry)restored.RankUps[0].Options[0].Requirements[0]).RequiredCount,
                Is.EqualTo(20));
        }

        [Test]
        public void SerializedSourceRejectsUnsupportedVersion()
        {
            var storage = new MemorySaveStorage();
            var saveService = new SaveService(new JsonSaveSerializer(), storage);
            Assert.That(
                saveService.TrySave(SerializedGameDataSource.DataKey, CreateDocument(6)),
                Is.True);
            var source = new SerializedGameDataSource(saveService);

            Assert.Throws<NotSupportedException>(() => source.Read());
        }

        private static GameDataDocument CreateDocument(int version)
        {
            return new GameDataDocument(
                version,
                Array.Empty<RankDocumentEntry>(),
                Array.Empty<CardDocumentEntry>(),
                Array.Empty<HeroDocumentEntry>(),
                Array.Empty<QuestDocumentEntry>(),
                new[]
                {
                    new RankUpDocumentEntry(
                        "hero",
                        "bronze",
                        "reward",
                        new[]
                        {
                            new RankUpOptionDocumentEntry(
                                "quest",
                                new RankUpRequirementDocumentEntry[]
                                {
                                    new QuestRankUpRequirementDocumentEntry("quest", "quest", 20, 10),
                                    new PaymentRankUpRequirementDocumentEntry(
                                        "soft-payment",
                                        new PaymentDocumentEntry("soft", 1))
                                })
                        })
                },
                Array.Empty<RewardDocumentEntry>());
        }
    }
}