using LL.Validation;
using LLEditor.Validation.Sources;

namespace LLEditor.Validation.References
{
    internal interface IProjectDataReferenceValidation
    {
        void Validate(
            ProjectDataSources sources,
            ValidationContext context);
    }
}