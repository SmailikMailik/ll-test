using System;
using LL.Validation;
using NUnit.Framework;

namespace LL.Tests.EditMode.Validation
{
    internal sealed class ValidationRunnerTests
    {
        [Test]
        public void RunAndIsValidReflectReportedErrors()
        {
            var result = ValidationRunner.Run(
                context => ValidationRules.NotEmpty(string.Empty, context, "value.required"));

            Assert.That(result.IsValid, Is.False);
            Assert.That(
                ValidationRunner.IsValid(context => ValidationRules.NotEmpty("value", context, "value.required")),
                Is.True);
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
    }
}