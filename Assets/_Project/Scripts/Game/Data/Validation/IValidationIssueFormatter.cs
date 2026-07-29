namespace LL.Game.Data.Validation
{
    internal interface IValidationIssueFormatter
    {
        string Format(ValidationIssue issue);
    }
}