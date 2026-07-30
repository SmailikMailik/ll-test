using System;
using LL.Presentation.Localization;
using LL.Presentation.Typography;
using R3;
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
        private TimeSpan _duration;
        private bool _hasDuration;

        [Inject]
        private void Construct(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        private void Start()
        {
            _localization.LocaleChanged.Subscribe(_ => OnLocaleChanged()).AddTo(this);
        }

        internal void SetDuration(TimeSpan duration)
        {
            _duration = duration;
            _hasDuration = true;
            RefreshText();
        }

        private void OnLocaleChanged()
        {
            if (_hasDuration)
                RefreshText();
        }

        private void RefreshText()
        {
            var hoursUnit = _localization.GetText(DurationLocalizationKeys.HoursUnit);
            var minutesUnit = _localization.GetText(DurationLocalizationKeys.MinutesUnit);
            var secondsUnit = _localization.GetText(DurationLocalizationKeys.SecondsUnit);
            _label.text = TextFormatter.Duration(_duration, hoursUnit, minutesUnit, secondsUnit);
        }
    }
}