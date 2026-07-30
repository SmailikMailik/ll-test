using System;
using LL.Game.Quests;
using LL.User.Snapshots;
using R3;

namespace LL.User.State.RankUp
{
    internal interface IUserRankUpQuest
    {
        QuestId QuestId { get; }
        long DeadlineUnixMilliseconds { get; }
        bool IsActive { get; }
        bool IsCompleted { get; }
        Observable<Unit> Changed { get; }

        TimeSpan GetRemainingTime();
        UserRankUpQuestSnapshot CreateSnapshot();
        bool TryStart(QuestId questId, TimeSpan duration);
        bool TryComplete();
        bool TryExpire();
        void ClearQuest();
    }
}