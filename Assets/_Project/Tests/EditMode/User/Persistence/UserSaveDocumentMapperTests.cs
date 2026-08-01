using System.Linq;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.Tests.EditMode.TestData;
using LL.User.Persistence;
using LL.User.Snapshots;
using NUnit.Framework;

namespace LL.Tests.EditMode.User.Persistence
{
    internal sealed class UserSaveDocumentMapperTests
    {
        [Test]
        public void DocumentRoundTripPreservesSnapshot()
        {
            var source = CreatePopulatedSnapshot();

            var document = UserSaveDocumentMapper.ToDocument(source);
            var restored = UserSaveDocumentMapper.ToSnapshot(document);

            AssertIdentityPreserved(source, restored);
            AssertHeroPreserved(source, restored);
            AssertItemsPreserved(source, restored);
        }

        private static UserSnapshot CreatePopulatedSnapshot()
        {
            return UserTestData.CreateSnapshot(
                experience: 25,
                rankUpAttempts: new[] { CreateQuestRankUpAttempt() },
                items: new[]
                {
                    new ItemAmount(ItemIds.Soft, 15),
                    new ItemAmount(ItemIds.Hard, 3)
                });
        }

        private static UserRankUpAttemptSnapshot CreateQuestRankUpAttempt()
        {
            return new UserRankUpAttemptSnapshot(
                new RankId("bronze"),
                new RankUpOptionId("quest"),
                new[]
                {
                    new UserRankUpQuestRequirementSnapshot(
                        new RankUpRequirementId("quest"),
                        5,
                        123456L)
                });
        }

        private static void AssertIdentityPreserved(UserSnapshot source, UserSnapshot restored)
        {
            Assert.That(restored.Identity.UserId, Is.EqualTo(source.Identity.UserId));
            Assert.That(restored.Identity.RegionCode, Is.EqualTo(source.Identity.RegionCode));
            Assert.That(restored.HeroSelection.HeroId, Is.EqualTo(source.HeroSelection.HeroId));
        }

        private static void AssertHeroPreserved(UserSnapshot source, UserSnapshot restored)
        {
            Assert.That(restored.Heroes.TryGetHero(new HeroId("hero"), out var restoredHero), Is.True);
            Assert.That(source.Heroes.TryGetHero(new HeroId("hero"), out var sourceHero), Is.True);
            Assert.That(restoredHero.Progress.RankId, Is.EqualTo(sourceHero.Progress.RankId));
            Assert.That(restoredHero.Progress.Experience, Is.EqualTo(sourceHero.Progress.Experience));

            AssertRankUpAttemptPreserved(sourceHero, restoredHero);
        }

        private static void AssertRankUpAttemptPreserved(
            UserHeroSnapshot sourceHero,
            UserHeroSnapshot restoredHero)
        {
            var sourceAttempt = sourceHero.RankUpAttempts.Single();
            var restoredAttempt = restoredHero.RankUpAttempts.Single();

            Assert.That(restoredAttempt.RankId, Is.EqualTo(sourceAttempt.RankId));
            Assert.That(restoredAttempt.OptionId, Is.EqualTo(sourceAttempt.OptionId));

            var sourceQuest = sourceAttempt.Quests.Single();
            var restoredQuest = restoredAttempt.Quests.Single();

            Assert.That(restoredQuest.RequirementId, Is.EqualTo(sourceQuest.RequirementId));
            Assert.That(restoredQuest.CurrentCount, Is.EqualTo(sourceQuest.CurrentCount));
            Assert.That(restoredQuest.DeadlineUnixMilliseconds, Is.EqualTo(sourceQuest.DeadlineUnixMilliseconds));
        }

        private static void AssertItemsPreserved(UserSnapshot source, UserSnapshot restored)
        {
            Assert.That(
                restored.Items.Amounts.Select(item => item.Id),
                Is.EqualTo(source.Items.Amounts.Select(item => item.Id)));
            Assert.That(
                restored.Items.Amounts.Select(item => item.Amount),
                Is.EqualTo(source.Items.Amounts.Select(item => item.Amount)));
        }
    }
}