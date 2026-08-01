using System;
using System.Linq;
using System.Text;
using LL.Game.Data.Persistence.Documents;
using LL.Infrastructure.Saving.Serialization;
using NUnit.Framework;

namespace LL.Tests.EditMode.Game.Data
{
    internal sealed class GameDataDocumentTests
    {
        private const int RequiredQuestCount = 20;

        [Test]
        public void RoundTripPreservesPolymorphicRequirements()
        {
            var serializer = new JsonSaveSerializer();
            var document = CreateDocument();

            Assert.That(serializer.TrySerialize(document, out var bytes), Is.True);

            var json = Encoding.UTF8.GetString(bytes);
            Assert.That(json, Does.Contain("\"Type\": \"quest\""));
            Assert.That(json, Does.Contain("\"Type\": \"payment\""));
            Assert.That(json, Does.Contain($"\"RequiredCount\": {RequiredQuestCount}"));
            Assert.That(json, Does.Not.Contain("RequiredAmount"));
            Assert.That(json, Does.Not.Contain("$type"));
            Assert.That(serializer.TryDeserialize(bytes, out GameDataDocument restored), Is.True);

            var restoredRequirements = restored.RankUps.Single().Options.Single().Requirements;
            var restoredQuest = restoredRequirements.OfType<QuestRankUpRequirementDocumentEntry>().Single();
            Assert.That(restored.Version, Is.EqualTo(GameDataDocument.CurrentVersion));
            Assert.That(restoredQuest.RequiredCount, Is.EqualTo(RequiredQuestCount));
        }

        private static GameDataDocument CreateDocument()
        {
            var requirements = new RankUpRequirementDocumentEntry[]
            {
                new QuestRankUpRequirementDocumentEntry("quest", "quest", RequiredQuestCount, 10),
                new PaymentRankUpRequirementDocumentEntry("soft-payment", new PaymentDocumentEntry("soft", 1))
            };
            var options = new[] { new RankUpOptionDocumentEntry("quest", requirements) };
            var rankUps = new[] { new RankUpDocumentEntry("hero", "bronze", "reward", options) };

            return new GameDataDocument(
                GameDataDocument.CurrentVersion,
                Array.Empty<RankDocumentEntry>(),
                Array.Empty<CardDocumentEntry>(),
                Array.Empty<HeroDocumentEntry>(),
                Array.Empty<QuestDocumentEntry>(),
                rankUps,
                Array.Empty<RewardDocumentEntry>());
        }
    }
}