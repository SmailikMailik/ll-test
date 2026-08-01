using System.Linq;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Quests;
using LL.Game.Ranks;
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
                new UserProgressSnapshot(new RankId("bronze"), 25),
                new UserRankUpQuestSnapshot(new QuestId("quest"), 123456L, true),
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
            Assert.That(restored.Progress.RankId, Is.EqualTo(source.Progress.RankId));
            Assert.That(restored.Progress.Experience, Is.EqualTo(source.Progress.Experience));
            Assert.That(restored.RankUpQuest.QuestId, Is.EqualTo(source.RankUpQuest.QuestId));
            Assert.That(restored.RankUpQuest.IsCompleted, Is.True);
            Assert.That(
                restored.Items.Amounts.Select(item => item.Id),
                Is.EqualTo(source.Items.Amounts.Select(item => item.Id)));
            Assert.That(
                restored.Items.Amounts.Select(item => item.Amount),
                Is.EqualTo(source.Items.Amounts.Select(item => item.Amount)));
        }
    }
}