using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;
using LL.Game.Ranks;
using LL.Game.RankUp;

namespace LL.User.Snapshots
{
    internal sealed class UserRankUpAttemptSnapshot
    {
        internal RankId RankId { get; }
        internal RankUpOptionId OptionId { get; }
        internal IReadOnlyList<UserRankUpQuestRequirementSnapshot> Quests { get; }

        internal UserRankUpAttemptSnapshot(
            RankId rankId,
            RankUpOptionId optionId,
            IEnumerable<UserRankUpQuestRequirementSnapshot> quests)
        {
            IdentifierValidator.EnsureValid(rankId, nameof(rankId));
            IdentifierValidator.EnsureValid(optionId, nameof(optionId));

            var questArray = quests?.ToArray() ?? Array.Empty<UserRankUpQuestRequirementSnapshot>();
            IdentifierCollectionValidator.EnsureValid(
                questArray,
                quest => quest.RequirementId,
                nameof(quests));

            RankId = rankId;
            OptionId = optionId;
            Quests = Array.AsReadOnly(questArray);
        }
    }
}