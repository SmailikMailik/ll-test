using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.Ranks;
using LL.Game.Ranks.Configuration;
using LL.User.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References
{
    internal sealed class UserProgressRankReferenceValidator : IProjectDataReferenceValidation
    {
        private const string RankExistsCode = "user-defaults.progress.rank.exists";
        private const string ExperienceCode = "user-defaults.progress.experience.non-negative";
        private const string ExperienceMaxCode = "user-defaults.progress.experience.maximum";
        private const string FinalRankExperienceCode = "user-defaults.progress.final-rank-experience.zero";

        public void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            var ranks = sources.GetSingle<RankCatalogConfig>();
            var userDefaults = sources.GetSingle<UserDefaultsConfig>();

            if (ranks == null || userDefaults == null)
                return;

            ValidateReferences(
                userDefaults.Heroes,
                ranks.Ranks,
                context
                    .At(AssetDatabase.GetAssetPath(userDefaults))
                    .At(nameof(UserDefaultsConfig.Heroes)));
        }

        private static void ValidateReferences(
            IReadOnlyList<UserHeroDefaultEntry> heroes,
            IReadOnlyList<RankEntry> ranks,
            ValidationContext context)
        {
            if (heroes is null ||
                ranks is null ||
                ranks.Count == 0)
            {
                return;
            }

            var rankIds = new HashSet<RankId>();

            for (var index = 0; index < ranks.Count; index++)
            {
                var rank = ranks[index];

                if (rank is not null && IdentifierValidator.IsValid(rank.Id))
                    rankIds.Add(rank.Id);
            }

            for (var index = 0; index < heroes.Count; index++)
            {
                var hero = heroes[index];

                if (hero is null || IdentifierValidator.IsValid(hero.RankId) is false)
                    continue;

                ValidateProgress(hero, ranks, rankIds, context.At(index));
            }
        }

        private static void ValidateProgress(
            UserHeroDefaultEntry hero,
            IReadOnlyList<RankEntry> ranks,
            ISet<RankId> rankIds,
            ValidationContext context)
        {
            if (ValidationRules.ReferenceExists(
                    hero.RankId,
                    rankIds,
                    context.At(nameof(UserHeroDefaultEntry.RankId)),
                    RankExistsCode) is false)
            {
                return;
            }

            var experienceContext = context.At(nameof(UserHeroDefaultEntry.Experience));

            if (ValidationRules.NonNegative(hero.Experience, experienceContext, ExperienceCode) is false)
                return;

            var rankIndex = FindRankIndex(ranks, hero.RankId);

            if (rankIndex + 1 < ranks.Count)
            {
                ValidationRules.LessThanOrEqual(
                    hero.Experience,
                    ranks[rankIndex + 1].RequiredExperience,
                    experienceContext,
                    ExperienceMaxCode);
                return;
            }

            ValidationRules.Equal(
                hero.Experience,
                0,
                experienceContext,
                FinalRankExperienceCode);
        }

        private static int FindRankIndex(
            IReadOnlyList<RankEntry> ranks,
            RankId rankId)
        {
            for (var index = 0; index < ranks.Count; index++)
            {
                if (ranks[index]?.Id.Equals(rankId) == true)
                    return index;
            }

            return -1;
        }
    }
}