using System;
using LL.Game.Heroes;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.User.Snapshots;
using R3;

namespace LL.User.State.RankUp
{
    internal interface IUserRankUpAttempts
    {
        Observable<Unit> Changed { get; }

        bool TryGetQuest(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementId requirementId,
            out UserRankUpQuestRequirementSnapshot quest);

        TimeSpan GetRemainingTime(UserRankUpQuestRequirementSnapshot quest);
    }
}