using System;
using LL.Game.Quests;

namespace LL.User.State.RankUp
{
    internal interface IUserRankUpQuestCommands
    {
        bool TryStart(QuestId questId, TimeSpan duration);
        bool TryComplete();
        bool TryExpire();
        void Clear();
    }
}