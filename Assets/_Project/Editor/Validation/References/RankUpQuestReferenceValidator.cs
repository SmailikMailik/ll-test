using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.RankUp.Configuration;
using LL.Game.Quests;
using LL.Game.Quests.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References
{
    internal sealed class RankUpQuestReferenceValidator : IProjectDataReferenceValidation
    {
        private const string QuestExistsCode = "rank-up.quest.exists";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var rankUps = sources.GetSingle<RankUpCatalogConfig>();
            var quests = sources.GetSingle<QuestCatalogConfig>();

            if (rankUps == null || quests == null)
                return;

            var questIds = new HashSet<QuestId>();

            foreach (var quest in quests.Quests)
            {
                if (quest is not null && IdentifierValidator.IsValid(quest.Id))
                    questIds.Add(quest.Id);
            }

            var rankUpEntries = rankUps.RankUps;
            var rankUpContext = context.At(AssetDatabase.GetAssetPath(rankUps));

            for (var index = 0; index < rankUpEntries.Count; index++)
            {
                var rankUp = rankUpEntries[index];

                if (rankUp?.Options is null)
                    continue;

                for (var optionIndex = 0; optionIndex < rankUp.Options.Count; optionIndex++)
                {
                    var option = rankUp.Options[optionIndex];

                    if (option?.Requirements is null)
                        continue;

                    for (var requirementIndex = 0; requirementIndex < option.Requirements.Count; requirementIndex++)
                    {
                        if (option.Requirements[requirementIndex] is not QuestRankUpRequirementEntry quest)
                            continue;

                        if (IdentifierValidator.IsValid(quest.QuestId) is false)
                            continue;

                        ValidationRules.ReferenceExists(
                            quest.QuestId,
                            questIds,
                            rankUpContext
                                .At(index)
                                .At(nameof(RankUpEntry.Options))
                                .At(optionIndex)
                                .At(nameof(RankUpOptionEntry.Requirements))
                                .At(requirementIndex)
                                .At(nameof(QuestRankUpRequirementEntry.QuestId)),
                            QuestExistsCode);
                    }
                }
            }
        }
    }
}