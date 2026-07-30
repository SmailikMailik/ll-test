using System;
using System.Linq;
using LL.Game.Data.Configuration;
using LL.Validation;

namespace LL.Game.Data.Sources
{
    internal sealed class ScriptableObjectGameDataSource : IGameDataSource
    {
        private readonly GameDataManifestConfig _manifest;

        internal ScriptableObjectGameDataSource(GameDataManifestConfig manifest)
        {
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        public GameDataDeclaration Read()
        {
            ValidationRunner.EnsureValid(_manifest);
            ValidationRunner.EnsureValid(_manifest.Ranks);
            ValidationRunner.EnsureValid(_manifest.Cards);
            ValidationRunner.EnsureValid(_manifest.Quests);
            ValidationRunner.EnsureValid(_manifest.RankPromotions);
            ValidationRunner.EnsureValid(_manifest.Rewards);

            return new GameDataDeclaration(
                _manifest.Ranks.Ranks.Select(rank =>
                    new RankDeclaration(rank.Id.Value, rank.RequiredExperience)),
                _manifest.Cards.Cards.Select(card =>
                    new CardDeclaration(card.Id.Value, card.ExperienceAmount)),
                _manifest.Quests.Quests.Select(quest =>
                    new QuestDeclaration(
                        quest.Id.Value,
                        quest.TitleLocalizationKey,
                        quest.DescriptionLocalizationKey)),
                _manifest.RankPromotions.Promotions.Select(promotion =>
                    new RankPromotionDeclaration(
                        promotion.RankId.Value,
                        promotion.QuestId.Value,
                        promotion.HeroLocalizationKey,
                        promotion.RequiredAmount,
                        promotion.DurationMinutes,
                        new PaymentDeclaration(
                            promotion.QuestPayment.ItemId.Value,
                            promotion.QuestPayment.Amount),
                        new PaymentDeclaration(
                            promotion.InstantPayment.ItemId.Value,
                            promotion.InstantPayment.Amount),
                        promotion.RewardId.Value)),
                _manifest.Rewards.Rewards.Select(reward =>
                    new RewardDeclaration(
                        reward.Id.Value,
                        reward.Items.Select(item =>
                            new ItemAmountDeclaration(item.Id.Value, item.Amount)))));
        }
    }
}