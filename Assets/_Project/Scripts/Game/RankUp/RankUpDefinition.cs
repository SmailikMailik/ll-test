using System;
using LL.Game.Identifiers;
using LL.Game.Payments;
using LL.Game.Ranks;
using LL.Game.Rewards;

namespace LL.Game.RankUp
{
    internal sealed class RankUpDefinition
    {
        internal RankId RankId { get; }
        internal RankUpQuest Quest { get; }
        internal Payment InstantPayment { get; }
        internal RewardId RewardId { get; }

        internal RankUpDefinition(
            RankId rankId,
            RankUpQuest quest,
            Payment instantPayment,
            RewardId rewardId)
        {
            IdentifierValidator.EnsureValid(rankId, nameof(rankId));
            IdentifierValidator.EnsureValid(rewardId, nameof(rewardId));

            RankId = rankId;
            Quest = quest ?? throw new ArgumentNullException(nameof(quest));
            InstantPayment = instantPayment;
            RewardId = rewardId;
        }
    }
}