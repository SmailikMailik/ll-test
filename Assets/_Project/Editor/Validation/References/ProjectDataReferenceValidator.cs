using LL.Validation;
using LLEditor.Validation.Sources;

namespace LLEditor.Validation.References
{
    internal sealed class ProjectDataReferenceValidator
    {
        private readonly IProjectDataReferenceValidation[] _validations =
        {
            new RankPromotionRankReferenceValidator(),
            new RankPromotionRewardReferenceValidator(),
            new UserProgressRankReferenceValidator(),
            new ProjectItemReferenceValidator()
        };

        internal void Validate(
            ProjectDataSources sources,
            ValidationContext context)
        {
            foreach (var validation in _validations)
                validation.Validate(sources, context);
        }
    }
}