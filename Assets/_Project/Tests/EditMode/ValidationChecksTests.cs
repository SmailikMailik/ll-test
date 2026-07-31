using System.Collections.Generic;
using LL.Validation;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LL.Tests.EditMode
{
    internal sealed class ValidationChecksTests
    {
        [Test]
        public void NullAndEmptyChecksAreComplementary()
        {
            Assert.That(ValidationChecks.IsNull<string>(null), Is.True);
            Assert.That(ValidationChecks.IsNotNull(string.Empty), Is.True);
            Assert.That(ValidationChecks.IsEmpty(" "), Is.True);
            Assert.That(ValidationChecks.IsNotEmpty("value"), Is.True);
            Assert.That(ValidationChecks.IsEmpty<int>(null), Is.True);
            Assert.That(ValidationChecks.IsNotEmpty(new[] { 1 }), Is.True);
        }

        [Test]
        public void NullChecksUseUnityDestroyedObjectSemantics()
        {
            var value = ScriptableObject.CreateInstance<TestUnityObject>();

            Assert.That(ValidationChecks.IsNotNull(value), Is.True);

            Object.DestroyImmediate(value);

            Assert.That(ValidationChecks.IsNull(value), Is.True);
            Assert.That(ValidationChecks.IsNotNull(value), Is.False);
        }

        [Test]
        public void StringShapeChecksAreComplementary()
        {
            Assert.That(ValidationChecks.IsTrimmed("value"), Is.True);
            Assert.That(ValidationChecks.IsNotTrimmed(" value "), Is.True);
            Assert.That(ValidationChecks.IsUppercase("VALUE"), Is.True);
            Assert.That(ValidationChecks.IsNotUppercase("Value"), Is.True);
            Assert.That(ValidationChecks.IsLowercase("value"), Is.True);
            Assert.That(ValidationChecks.IsNotLowercase("Value"), Is.True);
        }

        [Test]
        public void NumericChecksCoverZeroBoundary()
        {
            Assert.That(ValidationChecks.IsNegative(-1), Is.True);
            Assert.That(ValidationChecks.IsNonNegative(0), Is.True);
            Assert.That(ValidationChecks.IsPositive(1), Is.True);
            Assert.That(ValidationChecks.IsNonPositive(0), Is.True);
            Assert.That(ValidationChecks.IsLessThanOrEqual(2, 2), Is.True);
            Assert.That(ValidationChecks.IsGreaterThan(3, 2), Is.True);
        }

        [Test]
        public void EqualityChecksUseProvidedComparer()
        {
            Assert.That(ValidationChecks.AreEqual("VALUE", "value", System.StringComparer.OrdinalIgnoreCase), Is.True);
            Assert.That(ValidationChecks.AreNotEqual("VALUE", "value", System.StringComparer.Ordinal), Is.True);
        }

        [Test]
        public void EnumAndSetChecksAreComplementary()
        {
            var values = new HashSet<string> { "value" };

            Assert.That(ValidationChecks.IsDefined(TestValue.Supported), Is.True);
            Assert.That(ValidationChecks.IsUndefined((TestValue)byte.MaxValue), Is.True);
            Assert.That(ValidationChecks.Contains("value", values), Is.True);
            Assert.That(ValidationChecks.DoesNotContain("other", values), Is.True);
        }

        [Test]
        public void TryAddUniqueAddsOnlyNewValues()
        {
            var values = new HashSet<string>();

            Assert.That(ValidationChecks.TryAddUnique("value", values), Is.True);
            Assert.That(ValidationChecks.TryAddUnique("value", values), Is.False);
            Assert.That(values, Has.Count.EqualTo(1));
        }

        private enum TestValue : byte
        {
            Supported = 0
        }

        private sealed class TestUnityObject : ScriptableObject { }
    }
}