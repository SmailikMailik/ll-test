using System;
using System.Linq;
using LL.Game.Items;
using LL.Tests.EditMode.TestData;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LL.Tests.EditMode.User.Persistence
{
    internal sealed class UserSaveCoordinatorTests
    {
        private static readonly DateTimeOffset _now = DateTimeOffset.FromUnixTimeMilliseconds(1_000_000L);
        private static readonly TimeSpan _saveDelay = TimeSpan.FromMilliseconds(500d);

        private UserSaveCoordinatorTestContext _context;

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
            _context = null;
        }

        [Test]
        public void DebouncesChangesAndWritesLatestSnapshot()
        {
            _context = new UserSaveCoordinatorTestContext(_now);

            Assert.That(_context.Items.TryAdd(ItemIds.Soft, 1), Is.True);
            Assert.That(_context.Items.TryAdd(ItemIds.Soft, 2), Is.True);
            _context.Coordinator.Tick();
            Assert.That(_context.Repository.SaveCount, Is.Zero);

            _context.TimeProvider.Advance(_saveDelay);
            _context.Coordinator.Tick();

            Assert.That(_context.Repository.SaveCount, Is.EqualTo(1));
            Assert.That(
                _context.Repository.LastSnapshot.Items.Amounts
                    .Single(item => item.Id.Equals(ItemIds.Soft)).Amount,
                Is.EqualTo(3));
        }

        [Test]
        public void RetriesFailedWriteWithoutAnotherStateChange()
        {
            _context = new UserSaveCoordinatorTestContext(_now, failuresBeforeSuccess: 1);

            Assert.That(_context.Items.TryAdd(ItemIds.Soft, 1), Is.True);

            _context.TimeProvider.Advance(_saveDelay);
            LogAssert.Expect(LogType.Error, "User state could not be saved. Saving will retry automatically.");
            _context.Coordinator.Tick();
            Assert.That(_context.Repository.SaveCount, Is.EqualTo(1));

            _context.TimeProvider.Advance(_saveDelay);
            _context.Coordinator.Tick();
            Assert.That(_context.Repository.SaveCount, Is.EqualTo(2));
        }
    }
}