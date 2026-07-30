using System;
using LL.Presentation.Localization;
using LL.UI.Windows;
using LL.UI.Windows.Modal;
using VContainer;

namespace LL.Presentation.Quests
{
    internal sealed class ModalQuestCompletionConfirmation : IQuestCompletionConfirmation
    {
        private readonly WindowController _windowController;
        private readonly ILocalizationService _localization;

        [Inject]
        internal ModalQuestCompletionConfirmation(
            WindowController windowController,
            ILocalizationService localization)
        {
            _windowController = windowController ?? throw new ArgumentNullException(nameof(windowController));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        public void Confirm(Action onConfirmed)
        {
            _windowController.Show(new ModalWindowParameters
            (
                headerText: _localization.GetText(QuestLocalizationKeys.TestingTitle),
                messageText: _localization.GetText(QuestLocalizationKeys.TestingMessage),
                positiveText: _localization.GetText(QuestLocalizationKeys.CompleteAction),
                positiveCallback: onConfirmed,
                negativeText: _localization.GetText(QuestLocalizationKeys.CancelAction),
                closeActive: false
            ));
        }
    }
}