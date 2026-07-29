namespace LL.Game.Data.Validation
{
    internal interface IValidationReporter
    {
        void Report(ValidationResult result);
    }
}