using System;
using System.Collections.Generic;
using LL.Infrastructure.Collections;
using NUnit.Framework;

namespace LL.Tests.EditMode.Infrastructure.Collections
{
    internal sealed class EnumerableExtensionsTests
    {
        [Test]
        public void NullSourceReturnsEmptyList()
        {
            IEnumerable<int> source = null;

            Assert.That(source.ToReadOnlyCopy(), Is.Empty);
        }

        [Test]
        public void CopyPreservesOrderAndDoesNotTrackSourceMutations()
        {
            var source = new List<int> { 1, 2 };
            var copy = source.ToReadOnlyCopy();

            source[0] = 3;
            source.Add(4);

            Assert.That(copy, Is.EqualTo(new[] { 1, 2 }));
        }

        [Test]
        public void CopyRejectsMutation()
        {
            var copy = (IList<int>)new[] { 1, 2 }.ToReadOnlyCopy();

            Assert.Throws<NotSupportedException>(() => copy.Add(3));
        }
    }
}