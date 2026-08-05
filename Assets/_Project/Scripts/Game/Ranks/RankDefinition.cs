using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Ranks
{
    internal sealed class RankDefinition
    {
        private const string NumberCode = "rank.number.positive";
        private const string InitialExperienceCode = "rank-catalog.initial-experience.zero";
        private const string ExperienceCode = "rank-catalog.experience.positive";

        internal RankId Id { get; }
        internal int Number { get; }
        internal int RequiredExperience { get; }

        internal RankDefinition(
            RankId id,
            int number,
            int requiredExperience)
        {
            IdentifierValidator.EnsureValid(id, nameof(id));
            ValidationRunner.EnsureValid(
                context => ValidationRules.Positive(number, context, NumberCode),
                nameof(number));
            ValidationRunner.EnsureValid(
                context =>
                {
                    if (number == 1)
                    {
                        ValidationRules.Equal(
                            requiredExperience,
                            0,
                            context,
                            InitialExperienceCode);
                        return;
                    }

                    ValidationRules.Positive(
                        requiredExperience,
                        context,
                        ExperienceCode);
                },
                nameof(requiredExperience));

            Id = id;
            Number = number;
            RequiredExperience = requiredExperience;
        }
    }
}