using System;

namespace LL.Presentation.Quests
{
    internal interface IQuestCompletionConfirmation
    {
        void Confirm(Action onConfirmed);
    }
}