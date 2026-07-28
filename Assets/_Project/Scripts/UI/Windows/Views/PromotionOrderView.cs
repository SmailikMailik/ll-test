using System;
using System.Collections.Generic;
using LL.Game.Promotions;
using LL.Presentation.Localization;
using LL.Presentation.Orders;
using LL.UI.Controls.Buttons;
using LL.UI.Localization;
using LL.UI.Typography;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views
{
    [DisallowMultipleComponent]
    internal sealed class PromotionOrderView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _descriptionLabel;
        [SerializeField] private LocalizedDurationLabel _timeLabel;
        [SerializeField] private InteractiveButton _button;

        private const string CountVariable = "count";
        private const string TargetVariable = "target";
        private static readonly TimeSpan TimerTickInterval = TimeSpan.FromSeconds(1);

        internal Observable<Unit> Completed => _completed;
        internal bool IsCompleted => _state == OrderState.Completed;

        private readonly Subject<Unit> _completed = new();

        private IOrderCompletionConfirmation _completionConfirmation;
        private ILocalizationService _localization;

        private OrderState _state = OrderState.Available;
        private float _deadline;
        private int _displayedRemainingMinutes = -1;
        private bool _canAccept;
        private TimeSpan _duration;

        [Inject]
        private void Construct(
            IOrderCompletionConfirmation completionConfirmation,
            ILocalizationService localization)
        {
            _completionConfirmation = completionConfirmation ??
                throw new ArgumentNullException(nameof(completionConfirmation));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        private void Start()
        {
            _button.Clicked
                .Subscribe(_ => HandleClick())
                .AddTo(this);

            Observable
                .Interval(TimerTickInterval)
                .Where(_ => _state == OrderState.Active)
                .Subscribe(_ => TickTimer())
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _completed.Dispose();
        }

        internal void Refresh(
            RankPromotionRequirement requirement,
            TimeSpan duration,
            bool canAccept)
        {
            _duration = duration;
            RefreshText(requirement);
            RefreshTime();
            SetAvailable(canAccept);
        }

        internal void SetAvailable(bool canAccept)
        {
            _canAccept = canAccept;
            _button.SetInteractable(_canAccept && IsCompleted is false);
        }

        internal void Reset()
        {
            _state = OrderState.Available;
            _deadline = 0f;
            _displayedRemainingMinutes = -1;
            RefreshTime();
            SetAvailable(_canAccept);
        }

        private void RefreshText(RankPromotionRequirement requirement)
        {
            if (requirement == null)
                throw new ArgumentNullException(nameof(requirement));

            var target = _localization.GetText(requirement.TargetLocalizationKey);

            _titleLabel.text = _localization.GetText(requirement.TitleLocalizationKey);
            _descriptionLabel.text = _localization.GetText(
                requirement.DescriptionLocalizationKey,
                new Dictionary<string, object>
                {
                    [CountVariable] = TextFormatter.Number(requirement.RequiredAmount),
                    [TargetVariable] = target
                });
        }

        private void HandleClick()
        {
            if (_canAccept is false || IsCompleted)
                return;

            if (_state == OrderState.Available)
                StartOrder();

            _completionConfirmation.Confirm(CompleteOrder);
        }

        private void StartOrder()
        {
            _state = OrderState.Active;
            _deadline = Time.unscaledTime + Mathf.Max(0f, (float)_duration.TotalSeconds);
            _displayedRemainingMinutes = -1;
            RefreshTime();
        }

        private void CompleteOrder()
        {
            if (_state != OrderState.Active)
                return;

            _state = OrderState.Completed;
            _displayedRemainingMinutes = -1;
            RefreshTime();
            SetAvailable(_canAccept);
            _completed.OnNext(Unit.Default);
        }

        private void TickTimer()
        {
            var remainingSeconds = GetRemainingSeconds();

            if (remainingSeconds <= 0f)
            {
                Reset();
                return;
            }

            RefreshTime(remainingSeconds);
        }

        private void RefreshTime()
        {
            var remainingSeconds = _state switch
            {
                OrderState.Available => Mathf.Max(0f, (float)_duration.TotalSeconds),
                OrderState.Active => GetRemainingSeconds(),
                OrderState.Completed => 0f,
                _ => 0f
            };

            RefreshTime(remainingSeconds);
        }

        private void RefreshTime(float remainingSeconds)
        {
            var remainingMinutes = Mathf.CeilToInt(Mathf.Max(0f, remainingSeconds) / 60f);

            if (_displayedRemainingMinutes == remainingMinutes)
                return;

            _displayedRemainingMinutes = remainingMinutes;
            _timeLabel.SetDuration(TimeSpan.FromMinutes(remainingMinutes));
        }

        private float GetRemainingSeconds()
        {
            return Mathf.Max(0f, _deadline - Time.unscaledTime);
        }

        private enum OrderState : byte
        {
            Available = 0,
            Active = 1,
            Completed = 2
        }
    }
}