using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.Promotions.Configuration;
using LL.Game.Quests;
using LL.Game.Quests.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References
{
    internal sealed class RankPromotionQuestReferenceValidator : IProjectDataReferenceValidation
    {
        private const string QuestExistsCode = "rank-promotion.quest.exists";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var promotions = sources.GetSingle<RankPromotionCatalogConfig>();
            var quests = sources.GetSingle<QuestCatalogConfig>();

            if (promotions == null || quests == null)
                return;

            var questIds = new HashSet<QuestId>();

            foreach (var quest in quests.Quests)
            {
                if (quest != null && IdentifierValidator.IsValid(quest.Id))
                    questIds.Add(quest.Id);
            }

            var promotionEntries = promotions.Promotions;
            var promotionContext = context.At(AssetDatabase.GetAssetPath(promotions));

            for (var index = 0; index < promotionEntries.Count; index++)
            {
                var promotion = promotionEntries[index];

                if (promotion == null || IdentifierValidator.IsValid(promotion.QuestId) is false)
                    continue;

                ValidationRules.ReferenceExists(
                    promotion.QuestId,
                    questIds,
                    promotionContext.At(index).At(nameof(RankPromotionEntry.QuestId)),
                    QuestExistsCode);
            }
        }
    }
}