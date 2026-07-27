using System;
using System.Collections.Generic;
using LL.Presentation.Localization;
using LL.UI.Typography;
using LL.UI.Windows;
using LL.UI.Windows.Views;
using LL.Upgrades;
using VContainer;

namespace LL.Presentation.Upgrades
{
    internal sealed class ModalExperienceOverflowConfirmation : IExperienceOverflowConfirmation
    {
        private const string ExperienceVariable = "experience";

        private readonly WindowController _windowController;
        private readonly ILocalizationService _localization;

        [Inject]
        internal ModalExperienceOverflowConfirmation(
            WindowController windowController,
            ILocalizationService localization)
        {
            _windowController = windowController ?? throw new ArgumentNullException(nameof(windowController));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        public void Confirm(int lostExperience, Action onConfirmed, Action onRejected)
        {
            if (lostExperience <= 0)
            {
                onConfirmed?.Invoke();
                return;
            }

            _windowController.Show(
                new ModalWindowParameters(
                    headerText: _localization.GetText(ExperienceOverflowLocalizationKeys.Title),
                    messageText: _localization.GetText(
                        ExperienceOverflowLocalizationKeys.Message,
                        new Dictionary<string, object>
                        {
                            [ExperienceVariable] = TextFormatter.Number(lostExperience)
                        }),
                    positiveText: _localization.GetText(ExperienceOverflowLocalizationKeys.UseAction),
                    positiveCallback: onConfirmed,
                    negativeText: _localization.GetText(ExperienceOverflowLocalizationKeys.CancelAction),
                    negativeCallback: onRejected,
                    closeCallback: onRejected));
        }
    }
}
