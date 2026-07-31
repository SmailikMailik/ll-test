using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.Heroes.Configuration;
using LL.Game.Identifiers;
using LL.Game.RankUp.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References
{
    internal sealed class RankUpHeroReferenceValidator : IProjectDataReferenceValidation
    {
        private const string HeroExistsCode = "rank-up.hero.exists";

        public void Validate(ProjectDataSources sources, ValidationContext context)
        {
            var rankUps = sources.GetSingle<RankUpCatalogConfig>();
            var heroes = sources.GetSingle<HeroCatalogConfig>();

            if (rankUps == null || heroes == null)
                return;

            var heroIds = new HashSet<HeroId>();

            foreach (var hero in heroes.Heroes)
            {
                if (hero != null && IdentifierValidator.IsValid(hero.Id))
                    heroIds.Add(hero.Id);
            }

            var rankUpEntries = rankUps.RankUps;
            var rankUpContext = context.At(AssetDatabase.GetAssetPath(rankUps));

            for (var index = 0; index < rankUpEntries.Count; index++)
            {
                var rankUp = rankUpEntries[index];

                if (rankUp == null || IdentifierValidator.IsValid(rankUp.HeroId) is false)
                    continue;

                ValidationRules.ReferenceExists(
                    rankUp.HeroId,
                    heroIds,
                    rankUpContext.At(index).At(nameof(RankUpEntry.HeroId)),
                    HeroExistsCode);
            }
        }
    }
}