using System;
using LL.Presentation.Localization;
using LL.UI.Typography;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Localization
{
    [DisallowMultipleComponent]
    [AddComponentMenu("LL/UI/Localization/Localized Duration Label")]
    internal sealed class LocalizedDurationLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        private ILocalizationService _localization;

        [Inject]
        private void Construct(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        internal void SetDuration(TimeSpan duration)
        {
            var hoursUnit = _localization.GetText(DurationLocalizationKeys.HoursUnit);
            var minutesUnit = _localization.GetText(DurationLocalizationKeys.MinutesUnit);
            var secondsUnit = _localization.GetText(DurationLocalizationKeys.SecondsUnit);
            _label.text = TextFormatter.Duration(duration, hoursUnit, minutesUnit, secondsUnit);
        }
    }
}