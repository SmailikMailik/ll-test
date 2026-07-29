using LL.Validation;

namespace LL.Game.Ranks.Configuration
{
    internal sealed class RankCatalogConfigValidator : IDataValidator<RankExperienceRequirementEntry[]>
    {
        private const string RequirementsCode = "rank-catalog.requirements.not-empty";
        private const string EntryCode = "rank-catalog.entry.required";
        private const string RankSequenceCode = "rank-catalog.rank.sequential";
        private const string InitialExperienceCode = "rank-catalog.initial-experience.zero";
        private const string ExperienceCode = "rank-catalog.experience.positive";

        public void Validate(
            RankExperienceRequirementEntry[] requirements,
            ValidationContext context)
        {
            if (ValidationRules.NotEmpty(requirements, context, RequirementsCode) is false)
                return;

            for (var index = 0; index < requirements.Length; index++)
            {
                var requirement = requirements[index];
                var requirementContext = context.At(index);

                if (ValidationRules.NotNull(requirement, requirementContext, EntryCode) is false)
                    continue;

                var expectedRank = index + 1;

                if (requirement.Rank != expectedRank)
                {
                    requirementContext
                        .At("Rank")
                        .Report(
                            ValidationSeverity.Error,
                            RankSequenceCode,
                            $"Rank must be {expectedRank} at index {index}.");
                }

                var experienceContext = requirementContext.At("RequiredExperience");

                if (index == 0)
                {
                    if (requirement.RequiredExperience != 0)
                    {
                        experienceContext.Report(
                            ValidationSeverity.Error,
                            InitialExperienceCode,
                            "Rank 1 required experience must be zero.");
                    }

                    continue;
                }

                ValidationRules.Positive(
                    requirement.RequiredExperience,
                    experienceContext,
                    ExperienceCode);
            }
        }
    }
}