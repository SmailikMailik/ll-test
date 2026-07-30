using System;
using LL.User.Snapshots;

namespace LL.User.Persistence
{
    internal sealed class UserLoadResult
    {
        internal UserLoadStatus Status { get; }
        internal UserSnapshot Snapshot { get; }
        internal int DocumentVersion { get; }

        private UserLoadResult(UserLoadStatus status, UserSnapshot snapshot, int documentVersion)
        {
            Status = status;
            Snapshot = snapshot;
            DocumentVersion = documentVersion;
        }

        internal static UserLoadResult Loaded(UserSnapshot snapshot)
        {
            return new UserLoadResult(
                UserLoadStatus.Loaded,
                snapshot ?? throw new ArgumentNullException(nameof(snapshot)),
                0);
        }

        internal static UserLoadResult Failed(UserLoadStatus status, int documentVersion = 0)
        {
            if (status == UserLoadStatus.Loaded)
                throw new ArgumentException("A failed load result cannot have Loaded status.", nameof(status));

            return new UserLoadResult(status, null, documentVersion);
        }
    }
}