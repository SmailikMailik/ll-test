using System.Collections.Generic;
using System.Linq;
using LL.Validation;
using NUnit.Framework;

namespace LL.Tests.EditMode.Validation
{
    internal sealed class ValidationRulesTests
    {
        [Test]
        public void ValidValuesDoNotReportIssues()
        {
            var availableValues = new HashSet<string> { "value" };
            var usedValues = new HashSet<string>();
            var result = ValidationRunner.Run(
                context =>
                {
                    ValidationRules.NotNull("value", context, "not-null");
                    ValidationRules.NotEmpty("value", context, "not-empty");
                    ValidationRules.Trimmed("value", context, "trimmed");
                    ValidationRules.Uppercase("VALUE", context, "uppercase");
                    ValidationRules.Lowercase("value", context, "lowercase");
                    ValidationRules.NotEmpty(new[] { 1 }, context, "collection.not-empty");
                    ValidationRules.Positive(1, context, "positive");
                    ValidationRules.NonNegative(0, context, "non-negative");
                    ValidationRules.Equal(1, 1, context, "equal");
                    ValidationRules.NotEqual(1, 2, context, "not-equal");
                    ValidationRules.LessThanOrEqual(1, 1, context, "less-than-or-equal");
                    ValidationRules.DefinedEnum(TestValue.Supported, context, "enum.defined");
                    ValidationRules.ReferenceExists("value", availableValues, context, "reference.exists");
                    ValidationRules.TryAddUnique("value", usedValues, context, "value.unique");
                });

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Issues, Is.Empty);
        }

        [Test]
        public void InvalidValuesReportEveryRuleCode()
        {
            var availableValues = new HashSet<string>();
            var usedValues = new HashSet<string> { "value" };
            var result = ValidationRunner.Run(
                context =>
                {
                    ValidationRules.NotNull<string>(null, context, "not-null");
                    ValidationRules.NotEmpty(" ", context, "not-empty");
                    ValidationRules.Trimmed(" value ", context, "trimmed");
                    ValidationRules.Uppercase("Value", context, "uppercase");
                    ValidationRules.Lowercase("Value", context, "lowercase");
                    ValidationRules.NotEmpty(new int[0], context, "collection.not-empty");
                    ValidationRules.Positive(0, context, "positive");
                    ValidationRules.NonNegative(-1, context, "non-negative");
                    ValidationRules.Equal(1, 2, context, "equal");
                    ValidationRules.NotEqual(1, 1, context, "not-equal");
                    ValidationRules.LessThanOrEqual(2, 1, context, "less-than-or-equal");
                    ValidationRules.DefinedEnum((TestValue)byte.MaxValue, context, "enum.defined");
                    ValidationRules.ReferenceExists("value", availableValues, context, "reference.exists");
                    ValidationRules.TryAddUnique("value", usedValues, context, "value.unique");
                });

            var expectedCodes = new[]
            {
                "not-null",
                "not-empty",
                "trimmed",
                "uppercase",
                "lowercase",
                "collection.not-empty",
                "positive",
                "non-negative",
                "equal",
                "not-equal",
                "less-than-or-equal",
                "enum.defined",
                "reference.exists",
                "value.unique"
            };

            Assert.That(result.ErrorCount, Is.EqualTo(expectedCodes.Length));
            CollectionAssert.AreEquivalent(expectedCodes, result.Issues.Select(issue => issue.Code));
        }

        private enum TestValue : byte
        {
            Supported = 0
        }
    }
}