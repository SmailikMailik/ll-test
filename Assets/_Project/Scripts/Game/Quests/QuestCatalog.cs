using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;

namespace LL.Game.Quests
{
    internal sealed class QuestCatalog
    {
        private readonly IReadOnlyDictionary<QuestId, QuestDefinition> _questsById;

        internal QuestCatalog(IEnumerable<QuestDefinition> quests)
        {
            var entries = quests?.ToArray() ?? Array.Empty<QuestDefinition>();
            IdentifierCollectionValidator.EnsureValid(
                entries,
                quest => quest.Id,
                nameof(quests));

            _questsById = entries.ToDictionary(quest => quest.Id);
        }

        internal bool TryGetQuest(QuestId id, out QuestDefinition quest) =>
            _questsById.TryGetValue(id, out quest);
    }
}