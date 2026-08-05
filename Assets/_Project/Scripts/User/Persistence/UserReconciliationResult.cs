using System;
using LL.User.Snapshots;

namespace LL.User.Persistence
{
    internal sealed class UserReconciliationResult
    {
        internal UserReconciliationStatus Status { get; }
        internal UserSnapshot Snapshot { get; }

        private UserReconciliationResult(
            UserReconciliationStatus status,
            UserSnapshot snapshot)
        {
            Status = status;
            Snapshot = snapshot;
        }

        internal static UserReconciliationResult Unchanged(UserSnapshot snapshot)
        {
            return new UserReconciliationResult(
                UserReconciliationStatus.Unchanged,
                snapshot ?? throw new ArgumentNullException(nameof(snapshot)));
        }

        internal static UserReconciliationResult Changed(UserSnapshot snapshot)
        {
            return new UserReconciliationResult(
                UserReconciliationStatus.Changed,
                snapshot ?? throw new ArgumentNullException(nameof(snapshot)));
        }

        internal static UserReconciliationResult Incompatible()
        {
            return new UserReconciliationResult(UserReconciliationStatus.Incompatible, null);
        }
    }
}