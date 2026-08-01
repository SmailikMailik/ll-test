using System;
using System.Linq;
using LL.Validation;
using NUnit.Framework;

namespace LL.Tests.EditMode.Validation
{
    internal sealed class EnumValidatorTests
    {
        [Test]
        public void ValidValuePassesEveryFacadeOperation()
        {
            Assert.That(EnumValidator.Validate(TestValue.Supported).IsValid, Is.True);
            Assert.That(EnumValidator.IsValid(TestValue.Supported), Is.True);
            Assert.DoesNotThrow(() => EnumValidator.EnsureValid(TestValue.Supported, "value"));
        }

        [Test]
        public void UndefinedValueReportsCodeAndFailsGuard()
        {
            var value = (TestValue)byte.MaxValue;
            var result = EnumValidator.Validate(value);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Issues.Single().Code, Is.EqualTo("enum.undefined"));

            var exception = Assert.Throws<ArgumentException>(() => EnumValidator.EnsureValid(value, "value"));
            Assert.That(exception.ParamName, Is.EqualTo("value"));
        }

        private enum TestValue : byte
        {
            Supported = 0
        }
    }
}