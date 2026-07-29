namespace LL.Validation
{
    internal interface IValidationSource
    {
        void Validate(ValidationContext context);
    }
}