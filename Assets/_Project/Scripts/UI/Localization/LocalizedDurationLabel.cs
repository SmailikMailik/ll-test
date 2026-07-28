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

        private const string HoursUnitKey = "Common/units.hours_short";
        private const string MinutesUnitKey = "Common/units.minutes_short";

        private ILocalizationService _localization;

        [Inject]
        private void Construct(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        public void SetDuration(TimeSpan duration)
        {
            var hoursUnit = _localization.GetText(HoursUnitKey);
            var minutesUnit = _localization.GetText(MinutesUnitKey);
            _label.text = TextFormatter.Duration(duration, hoursUnit, minutesUnit);
        }
    }
}