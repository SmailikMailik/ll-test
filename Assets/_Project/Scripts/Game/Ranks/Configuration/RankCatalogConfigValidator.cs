using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Ranks.Configuration
{
    internal sealed class RankCatalogConfigValidator : IDataValidator<RankEntry[]>
    {
        private const string EntriesCode = "rank-catalog.entries.not-empty";
        private const string InitialExperienceCode = "rank-catalog.initial-experience.zero";
        private const string ExperienceCode = "rank-catalog.experience.positive";

        public void Validate(
            RankEntry[] ranks,
            ValidationContext context)
        {
            if (ValidationRules.NotEmpty(ranks, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(
                ranks,
                rank => rank.Id,
                context);

            for (var index = 0; index < ranks.Length; index++)
            {
                var rank = ranks[index];

                if (rank == null)
                    continue;

                var rankContext = context.At(index);
                var experienceContext = rankContext.At(
                    nameof(RankEntry.RequiredExperience));

                if (index == 0)
                {
                    if (rank.RequiredExperience != 0)
                    {
                        experienceContext.Report(
                            ValidationSeverity.Error,
                            InitialExperienceCode,
                            "Rank 1 required experience must be zero.");
                    }

                    continue;
                }

                ValidationRules.Positive(
                    rank.RequiredExperience,
                    experienceContext,
                    ExperienceCode);
            }
        }
    }
}