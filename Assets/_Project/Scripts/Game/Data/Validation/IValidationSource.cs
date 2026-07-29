namespace LL.Game.Data.Validation
{
    internal interface IValidationSource
    {
        void Validate(ValidationContext context);
    }
}