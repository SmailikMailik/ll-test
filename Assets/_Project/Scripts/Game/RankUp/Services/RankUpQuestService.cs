using System;
using LL.User.State.RankUp;
using VContainer;

namespace LL.Game.RankUp.Services
{
    internal sealed class RankUpQuestService : IRankUpQuestService
    {
        private readonly IUserRankUpQuestCommands _commands;

        [Inject]
        internal RankUpQuestService(IUserRankUpQuestCommands commands)
        {
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public bool TryStart(RankUpQuest quest)
        {
            if (quest is null)
                throw new ArgumentNullException(nameof(quest));

            return _commands.TryStart(quest.QuestId, quest.Duration);
        }

        public bool TryComplete() => _commands.TryComplete();
        public bool TryExpire() => _commands.TryExpire();
        public void Clear() => _commands.Clear();
    }
}