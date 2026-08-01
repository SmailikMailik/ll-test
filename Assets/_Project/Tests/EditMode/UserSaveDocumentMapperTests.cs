using System.Linq;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.User.Persistence;
using LL.User.Snapshots;
using NUnit.Framework;

namespace LL.Tests.EditMode
{
    internal sealed class UserSaveDocumentMapperTests
    {
        [Test]
        public void DocumentRoundTripPreservesSnapshot()
        {
            var source = new UserSnapshot(
                new UserIdentitySnapshot("test-user", "RU"),
                new UserHeroSelectionSnapshot(new HeroId("hero")),
                new UserHeroesSnapshot(
                    new[]
                    {
                        new UserHeroSnapshot(
                            new HeroId("hero"),
                            new UserProgressSnapshot(new RankId("bronze"), 25),
                            new[]
                            {
                                new UserRankUpAttemptSnapshot(
                                    new RankId("bronze"),
                                    new RankUpOptionId("quest"),
                                    new[]
                                    {
                                        new UserRankUpQuestRequirementSnapshot(
                                            new RankUpRequirementId("quest"),
                                            5,
                                            123456L)
                                    })
                            })
                    }),
                new UserItemsSnapshot(
                    new[]
                    {
                        new ItemAmount(ItemIds.Soft, 15),
                        new ItemAmount(ItemIds.Hard, 3)
                    }));

            var document = UserSaveDocumentMapper.ToDocument(source);
            var restored = UserSaveDocumentMapper.ToSnapshot(document);

            Assert.That(restored.Identity.UserId, Is.EqualTo(source.Identity.UserId));
            Assert.That(restored.Identity.RegionCode, Is.EqualTo(source.Identity.RegionCode));
            Assert.That(restored.HeroSelection.HeroId, Is.EqualTo(source.HeroSelection.HeroId));
            Assert.That(restored.Heroes.TryGetHero(new HeroId("hero"), out var restoredHero), Is.True);
            Assert.That(source.Heroes.TryGetHero(new HeroId("hero"), out var sourceHero), Is.True);
            Assert.That(restoredHero.Progress.RankId, Is.EqualTo(sourceHero.Progress.RankId));
            Assert.That(restoredHero.Progress.Experience, Is.EqualTo(sourceHero.Progress.Experience));
            Assert.That(restoredHero.RankUpAttempts[0].OptionId, Is.EqualTo(new RankUpOptionId("quest")));
            Assert.That(restoredHero.RankUpAttempts[0].Quests[0].CurrentCount, Is.EqualTo(5));
            Assert.That(
                restored.Items.Amounts.Select(item => item.Id),
                Is.EqualTo(source.Items.Amounts.Select(item => item.Id)));
            Assert.That(
                restored.Items.Amounts.Select(item => item.Amount),
                Is.EqualTo(source.Items.Amounts.Select(item => item.Amount)));
        }
    }
}