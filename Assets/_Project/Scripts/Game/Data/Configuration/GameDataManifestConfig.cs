using System;
using LL.Game.Cards.Configuration;
using LL.Game.RankUp.Configuration;
using LL.Game.Quests.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using LL.Validation;
using UnityEngine;

namespace LL.Game.Data.Configuration
{
    [CreateAssetMenu(fileName = nameof(GameDataManifestConfig), menuName = CreationPath)]
    internal sealed class GameDataManifestConfig : ScriptableObject, IValidationSource
    {
        [SerializeField] private RankCatalogConfig _ranks;
        [SerializeField] private CardCatalogConfig _cards;
        [SerializeField] private QuestCatalogConfig _quests;
        [SerializeField] private RankUpCatalogConfig _rankUps;
        [SerializeField] private RewardCatalogConfig _rewards;

        internal const string CreationPath = "LL/Game Data/Game Data Manifest";

        internal RankCatalogConfig Ranks => _ranks != null ? _ranks : throw Missing(nameof(_ranks));
        internal CardCatalogConfig Cards => _cards != null ? _cards : throw Missing(nameof(_cards));
        internal QuestCatalogConfig Quests => _quests != null ? _quests : throw Missing(nameof(_quests));
        internal RankUpCatalogConfig RankUps =>
            _rankUps != null ? _rankUps : throw Missing(nameof(_rankUps));
        internal RewardCatalogConfig Rewards => _rewards != null ? _rewards : throw Missing(nameof(_rewards));

        private static InvalidOperationException Missing(string fieldName)
        {
            return new InvalidOperationException($"Game data manifest field '{fieldName}' is not assigned.");
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            ValidationRules.NotNull(_ranks, context.At(nameof(_ranks)), "game-data.manifest.ranks.required");
            ValidationRules.NotNull(_cards, context.At(nameof(_cards)), "game-data.manifest.cards.required");
            ValidationRules.NotNull(_quests, context.At(nameof(_quests)), "game-data.manifest.quests.required");
            ValidationRules.NotNull(
                _rankUps,
                context.At(nameof(_rankUps)),
                "game-data.manifest.rank-ups.required");
            ValidationRules.NotNull(_rewards, context.At(nameof(_rewards)), "game-data.manifest.rewards.required");
        }
    }
}