using System;
using System.Linq;
using LL.Game.Identifiers;
using NUnit.Framework;

namespace LL.Tests.EditMode.Game.Identifiers
{
    internal sealed class IdentifierCollectionValidatorTests
    {
        [Test]
        public void ValidUniqueEntriesPassEveryFacadeOperation()
        {
            var entries = new[]
            {
                new TestEntry("first"),
                new TestEntry("second")
            };

            Assert.That(IdentifierCollectionValidator.Validate(entries, entry => entry.Id).IsValid, Is.True);
            Assert.That(IdentifierCollectionValidator.IsValid(entries, entry => entry.Id), Is.True);
            Assert.DoesNotThrow(() => IdentifierCollectionValidator.EnsureValid(entries, entry => entry.Id, "entries"));
        }

        [Test]
        public void InvalidEntriesReportMissingMalformedAndDuplicateIdentifiers()
        {
            var entries = new[]
            {
                null,
                new TestEntry("Duplicate"),
                new TestEntry("duplicate")
            };
            var result = IdentifierCollectionValidator.Validate(entries, entry => entry.Id);
            var codes = result.Issues.Select(issue => issue.Code).ToArray();

            Assert.That(codes, Does.Contain("identifier.entry.required"));
            Assert.That(codes, Does.Contain("identifier.lowercase"));
            Assert.That(codes, Does.Contain("identifier.duplicate"));
        }

        [Test]
        public void EnsureValidPreservesParameterName()
        {
            var entries = new[] { new TestEntry("duplicate"), new TestEntry("duplicate") };
            var exception = Assert.Throws<ArgumentException>(
                () => IdentifierCollectionValidator.EnsureValid(entries, entry => entry.Id, "entries"));

            Assert.That(exception.ParamName, Is.EqualTo("entries"));
        }

        private sealed class TestEntry
        {
            internal TestIdentifier Id { get; }

            internal TestEntry(string id)
            {
                Id = new TestIdentifier(id);
            }
        }
    }
}