namespace LL.Validation.Reporting
{
    internal interface IValidationIssueFormatter
    {
        string Format(ValidationIssue issue);
    }
}