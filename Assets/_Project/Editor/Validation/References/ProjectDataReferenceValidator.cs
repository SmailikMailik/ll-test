using LL.Validation;
using LLEditor.Validation.References.Items;
using LLEditor.Validation.Sources;

namespace LLEditor.Validation.References
{
    internal sealed class ProjectDataReferenceValidator
    {
        private readonly IProjectDataReferenceValidation[] _validations =
        {
            new RankUpQuestReferenceValidator(),
            new RankUpHeroReferenceValidator(),
            new RankUpRankReferenceValidator(),
            new RankUpRewardReferenceValidator(),
            new UserProgressRankReferenceValidator(),
            new BuiltInUserItemReferenceValidator(),
            new CardItemReferenceValidator(),
            new RankUpPaymentReferenceValidator(),
            new RewardItemReferenceValidator(),
            new HeroPresentationReferenceValidator()
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