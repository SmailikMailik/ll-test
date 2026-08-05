using System;

namespace LL.User.Persistence.Documents
{
    internal sealed class UserRankUpAttemptDocumentEntry
    {
        public string RankId { get; }
        public string OptionId { get; }
        public UserRankUpQuestRequirementDocumentEntry[] Quests { get; }

        public UserRankUpAttemptDocumentEntry(
            string rankId,
            string optionId,
            UserRankUpQuestRequirementDocumentEntry[] quests)
        {
            RankId = rankId;
            OptionId = optionId;
            Quests = quests ?? Array.Empty<UserRankUpQuestRequirementDocumentEntry>();
        }
    }
}