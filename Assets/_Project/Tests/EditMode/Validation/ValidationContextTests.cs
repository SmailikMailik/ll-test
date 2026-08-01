using LL.Validation;
using NUnit.Framework;

namespace LL.Tests.EditMode.Validation
{
    internal sealed class ValidationContextTests
    {
        [Test]
        public void ContextBuildsNestedIssuePath()
        {
            var result = new ValidationResult();
            var context = new ValidationContext(result);

            context
                .At("Ranks")
                .At(2)
                .At("Id")
                .Report(ValidationSeverity.Warning, "rank.id.warning", "Warning.");

            Assert.That(result.Issues, Has.Count.EqualTo(1));
            Assert.That(result.Issues[0].Path, Is.EqualTo("Ranks[2].Id"));
            Assert.That(result.WarningCount, Is.EqualTo(1));
            Assert.That(result.IsValid, Is.True);
        }
    }
}