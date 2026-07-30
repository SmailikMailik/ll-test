using System;
using LL.Game.Quests;
using LL.User.Snapshots;
using R3;
using VContainer;

namespace LL.User.State.Promotions
{
    internal sealed class UserPromotionQuest : IUserPromotionQuest, IDisposable
    {
        private const long NoDeadline = 0L;

        public QuestId QuestId => _questId;
        public long DeadlineUnixMilliseconds => _deadlineUnixMilliseconds;
        public bool IsActive => HasQuest && IsCompleted is false && GetRemainingTime() > TimeSpan.Zero;
        public bool IsCompleted => _isCompleted;
        public Observable<Unit> Changed => _changed;

        private readonly Subject<Unit> _changed = new();

        private QuestId _questId;
        private long _deadlineUnixMilliseconds;
        private bool _isCompleted;

        private bool HasQuest =>
            string.IsNullOrWhiteSpace(_questId.Value) is false &&
            _deadlineUnixMilliseconds > NoDeadline;

        [Inject]
        internal UserPromotionQuest(UserPromotionQuestSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            _questId = snapshot.QuestId;
            _deadlineUnixMilliseconds = snapshot.DeadlineUnixMilliseconds;
            _isCompleted = snapshot.IsCompleted;
        }

        public TimeSpan GetRemainingTime()
        {
            if (HasQuest is false)
                return TimeSpan.Zero;

            var remainingMilliseconds = _deadlineUnixMilliseconds - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return TimeSpan.FromMilliseconds(Math.Max(0L, remainingMilliseconds));
        }

        public UserPromotionQuestSnapshot CreateSnapshot()
        {
            return new UserPromotionQuestSnapshot(
                QuestId,
                DeadlineUnixMilliseconds,
                IsCompleted);
        }

        public bool TryStart(QuestId questId, TimeSpan duration)
        {
            if (string.IsNullOrWhiteSpace(questId.Value) ||
                duration <= TimeSpan.Zero ||
                IsActive ||
                IsCompleted)
                return false;

            _questId = questId;
            _deadlineUnixMilliseconds = DateTimeOffset.UtcNow.Add(duration).ToUnixTimeMilliseconds();
            _isCompleted = false;
            NotifyChanged();
            return true;
        }

        public bool TryComplete()
        {
            if (IsActive is false)
            {
                TryExpire();
                return false;
            }

            _isCompleted = true;
            NotifyChanged();
            return true;
        }

        public bool TryExpire()
        {
            if (HasQuest is false || IsCompleted || GetRemainingTime() > TimeSpan.Zero)
                return false;

            ClearQuest();
            return true;
        }

        public void ClearQuest()
        {
            if (HasQuest is false && IsCompleted is false)
                return;

            _questId = default;
            _deadlineUnixMilliseconds = NoDeadline;
            _isCompleted = false;
            NotifyChanged();
        }

        public void Dispose()
        {
            _changed.Dispose();
        }

        private void NotifyChanged()
        {
            _changed.OnNext(Unit.Default);
        }
    }
}