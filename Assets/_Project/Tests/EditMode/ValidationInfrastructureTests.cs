using System;
using System.Collections.Generic;
using LL.Validation;
using NUnit.Framework;

namespace LL.Tests.EditMode
{
    internal sealed class ValidationInfrastructureTests
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

        [Test]
        public void RunnerCollectsEveryReportedError()
        {
            var result = ValidationRunner.Run(
                context =>
                {
                    ValidationRules.NotEmpty(string.Empty, context.At("Name"), "name.required");
                    ValidationRules.NonNegative(-1, context.At("Amount"), "amount.non-negative");
                });

            Assert.That(result.ErrorCount, Is.EqualTo(2));
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void EnsureValidFormatsEveryError()
        {
            var exception = Assert.Throws<ArgumentException>(
                () => ValidationRunner.EnsureValid(
                    context =>
                    {
                        ValidationRules.NotEmpty(string.Empty, context.At("Name"), "name.required");
                        ValidationRules.Positive(0, context.At("Amount"), "amount.positive");
                    },
                    "declaration"));

            Assert.That(exception.ParamName, Is.EqualTo("declaration"));
            Assert.That(exception.Message, Does.Contain("[Error] name.required at 'Name': Value must not be empty."));
            Assert.That(
                exception.Message,
                Does.Contain("[Error] amount.positive at 'Amount': Value must be greater than zero."));
        }

        [Test]
        public void TryAddUniqueAddsFirstValueAndReportsDuplicate()
        {
            var usedValues = new HashSet<string>(StringComparer.Ordinal);
            var result = new ValidationResult();
            var context = new ValidationContext(result);

            var firstWasAdded = ValidationRules.TryAddUnique("value", usedValues, context.At(0), "value.unique");
            var duplicateWasAdded = ValidationRules.TryAddUnique("value", usedValues, context.At(1), "value.unique");

            Assert.That(firstWasAdded, Is.True);
            Assert.That(duplicateWasAdded, Is.False);
            Assert.That(usedValues, Has.Count.EqualTo(1));
            Assert.That(result.ErrorCount, Is.EqualTo(1));
            Assert.That(result.Issues[0].Path, Is.EqualTo("[1]"));
        }
    }
}