using System;
using LL.Game.Quests;
using R3;

namespace LL.User.State.Promotions
{
    internal interface IUserPromotionQuest
    {
        QuestId QuestId { get; }
        long DeadlineUnixMilliseconds { get; }
        bool IsActive { get; }
        bool IsCompleted { get; }
        Observable<Unit> Changed { get; }

        TimeSpan GetRemainingTime();
        bool TryStart(QuestId questId, TimeSpan duration);
        bool TryComplete();
        bool TryExpire();
        void ClearQuest();
    }
}