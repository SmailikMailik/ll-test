using System;
using LL.Game.Promotions;
using R3;
using VContainer;

namespace LL.User.Core.Promotions
{
    internal sealed class UserPromotionOrder : IUserPromotionOrder, IDisposable
    {
        private const long NoDeadline = 0L;

        public PromotionRequirementId RequirementId => _requirementId;
        public long DeadlineUnixMilliseconds => _deadlineUnixMilliseconds;
        public bool IsActive => HasOrder && IsCompleted is false && GetRemainingTime() > TimeSpan.Zero;
        public bool IsCompleted => _isCompleted;
        public Observable<Unit> Changed => _changed;

        private readonly Subject<Unit> _changed = new();

        private PromotionRequirementId _requirementId;
        private long _deadlineUnixMilliseconds;
        private bool _isCompleted;

        private bool HasOrder => _requirementId.IsEmpty is false && _deadlineUnixMilliseconds > NoDeadline;

        [Inject]
        internal UserPromotionOrder(PromotionOrderInitialData initialData)
        {
            if (initialData == null)
                throw new ArgumentNullException(nameof(initialData));

            _requirementId = initialData.RequirementId;
            _deadlineUnixMilliseconds = initialData.DeadlineUnixMilliseconds;
            _isCompleted = initialData.IsCompleted;
        }

        public TimeSpan GetRemainingTime()
        {
            if (HasOrder is false)
                return TimeSpan.Zero;

            var remainingMilliseconds = _deadlineUnixMilliseconds - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return TimeSpan.FromMilliseconds(Math.Max(0L, remainingMilliseconds));
        }

        public bool TryStart(PromotionRequirementId requirementId, TimeSpan duration)
        {
            if (requirementId.IsEmpty || duration <= TimeSpan.Zero || IsActive || IsCompleted)
                return false;

            _requirementId = requirementId;
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
            if (HasOrder is false || IsCompleted || GetRemainingTime() > TimeSpan.Zero)
                return false;

            Reset();
            return true;
        }

        public void Reset()
        {
            if (HasOrder is false && IsCompleted is false)
                return;

            _requirementId = default;
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