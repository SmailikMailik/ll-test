using System;
using System.Linq;
using LL.Game.Identifiers;
using NUnit.Framework;

namespace LL.Tests.EditMode.Game.Identifiers
{
    internal sealed class IdentifierValidatorTests
    {
        [Test]
        public void LowercaseTrimmedIdentifierPassesEveryFacadeOperation()
        {
            var id = new TestIdentifier("valid-id");

            Assert.That(IdentifierValidator.Validate(id).IsValid, Is.True);
            Assert.That(IdentifierValidator.IsValid(id), Is.True);
            Assert.DoesNotThrow(() => IdentifierValidator.EnsureValid(id, "id"));
        }

        [TestCase(null, "identifier.empty")]
        [TestCase("", "identifier.empty")]
        [TestCase(" value ", "identifier.trimmed")]
        [TestCase("Value", "identifier.lowercase")]
        public void InvalidIdentifierReportsExpectedCode(string value, string expectedCode)
        {
            var result = IdentifierValidator.Validate(new TestIdentifier(value));

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Issues.Select(issue => issue.Code), Does.Contain(expectedCode));
        }

        [Test]
        public void EnsureValidPreservesParameterName()
        {
            var exception = Assert.Throws<ArgumentException>(
                () => IdentifierValidator.EnsureValid(new TestIdentifier("Invalid"), "id"));

            Assert.That(exception.ParamName, Is.EqualTo("id"));
        }
    }
}