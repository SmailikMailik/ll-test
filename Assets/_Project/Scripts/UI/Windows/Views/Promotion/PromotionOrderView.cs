using System;
using System.Collections.Generic;
using LL.Game.Promotions;
using LL.Presentation.Localization;
using LL.Presentation.Orders;
using LL.Presentation.Promotions;
using LL.UI.Controls;
using LL.UI.Localization;
using LL.UI.Typography;
using LL.User.State.Promotions;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views.Promotion
{
    [DisallowMultipleComponent]
    internal sealed class PromotionOrderView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _descriptionLabel;
        [SerializeField] private LocalizedDurationLabel _timeLabel;
        [SerializeField] private TMP_Text _buttonLabel;
        [SerializeField] private InteractiveButton _button;

        private const string CountVariable = "count";
        private const string TargetVariable = "target";
        private const string UnlockVariable = "unlock";

        private static readonly TimeSpan _timerTickInterval = TimeSpan.FromSeconds(1);

        internal Observable<Unit> Completed => _completed;
        internal bool IsCompleted => _state == OrderState.Completed;

        private readonly Subject<Unit> _completed = new();

        private IOrderCompletionConfirmation _completionConfirmation;
        private ILocalizationService _localization;
        private IUserPromotionOrder _promotionOrder;

        private OrderState _state = OrderState.Available;
        private RankPromotionRequirement _requirement;
        private PromotionRequirementId _requirementId;
        private int _displayedRemainingSeconds = -1;
        private bool _canAccept;
        private TimeSpan _duration;

        [Inject]
        private void Construct(
            IOrderCompletionConfirmation completionConfirmation,
            ILocalizationService localization,
            IUserPromotionOrder promotionOrder)
        {
            _completionConfirmation = completionConfirmation ?? throw new ArgumentNullException(nameof(completionConfirmation));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _promotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
        }

        private void Start()
        {
            _completed.AddTo(this);
            _button.Clicked.Subscribe(_ => OnButtonClicked()).AddTo(this);
            _localization.LocaleChanged.Subscribe(_ => OnLocaleChanged()).AddTo(this);

            Observable
                .Interval(_timerTickInterval)
                .Where(_ => _state == OrderState.Active)
                .Subscribe(_ => TickTimer())
                .AddTo(this);
        }

        internal void Refresh(
            RankPromotionRequirement requirement,
            TimeSpan duration,
            bool canAccept)
        {
            _requirement = requirement ?? throw new ArgumentNullException(nameof(requirement));
            _requirementId = requirement.Id;
            _duration = duration;
            _canAccept = canAccept;
            RestoreState();
            RefreshText(requirement);
            RefreshState();
        }

        internal void SetAvailable(bool canAccept)
        {
            _canAccept = canAccept;
            RefreshButtonAvailability();
        }

        internal void ClearOrder()
        {
            _promotionOrder.ClearOrder();
            SetState(OrderState.Available);
        }

        private void RefreshText(RankPromotionRequirement requirement)
        {
            var unlockText = TextTags.Style(
                _localization.GetText(RankPromotionLocalizationKeys.Unlock),
                TextStyle.Accent);
            var countText = TextTags.Style(
                TextFormatter.Number(requirement.RequiredAmount),
                TextStyle.Accent);

            _titleLabel.text = _localization.GetText(
                requirement.TitleLocalizationKey,
                new Dictionary<string, object>
                {
                    [UnlockVariable] = unlockText
                });
            _descriptionLabel.text = _localization.GetText(
                requirement.DescriptionLocalizationKey,
                new Dictionary<string, object>
                {
                    [CountVariable] = countText,
                    [TargetVariable] = _localization.GetText(requirement.TargetLocalizationKey)
                });
        }

        private void RefreshButtonText()
        {
            var localizationKey = _state switch
            {
                OrderState.Available => OrderLocalizationKeys.AcceptAction,
                OrderState.Active => OrderLocalizationKeys.CompleteAction,
                _ => OrderLocalizationKeys.CompletedLabel
            };

            _buttonLabel.text = _localization.GetText(localizationKey);
        }

        private void OnButtonClicked()
        {
            if (_canAccept is false || IsCompleted)
                return;

            if (_state == OrderState.Available && StartOrder() is false)
                return;

            _completionConfirmation.Confirm(CompleteOrder);
        }

        private bool StartOrder()
        {
            if (_promotionOrder.TryStart(_requirementId, _duration) is false)
                return false;

            SetState(OrderState.Active);
            return true;
        }

        private void CompleteOrder()
        {
            if (_state != OrderState.Active || _promotionOrder.TryComplete() is false)
                return;

            SetState(OrderState.Completed);
            _completed.OnNext(Unit.Default);
        }

        private void SetState(OrderState state)
        {
            _state = state;
            RefreshState();
        }

        private void RestoreState()
        {
            if (_promotionOrder.RequirementId.Equals(_requirementId) is false)
                _promotionOrder.ClearOrder();

            _promotionOrder.TryExpire();

            if (_promotionOrder.IsCompleted)
                _state = OrderState.Completed;
            else if (_promotionOrder.IsActive)
                _state = OrderState.Active;
            else
                _state = OrderState.Available;
        }

        private void RefreshState()
        {
            _displayedRemainingSeconds = -1;
            var showTime = _state != OrderState.Completed;
            _timeLabel.gameObject.SetActive(showTime);

            if (showTime)
                RefreshTime();

            RefreshButtonText();
            RefreshButtonAvailability();
        }

        private void RefreshButtonAvailability()
        {
            _button.SetInteractable(_canAccept && IsCompleted is false);
        }

        private void OnLocaleChanged()
        {
            if (_requirement != null)
                RefreshText(_requirement);

            RefreshButtonText();
        }

        private void TickTimer()
        {
            var remainingSeconds = GetRemainingSeconds();

            if (remainingSeconds <= 0f)
            {
                ClearOrder();
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
                _ => 0f
            };

            RefreshTime(remainingSeconds);
        }

        private void RefreshTime(float remainingSeconds)
        {
            var displayedRemainingSeconds = Mathf.CeilToInt(Mathf.Max(0f, remainingSeconds));

            if (_displayedRemainingSeconds == displayedRemainingSeconds)
                return;

            _displayedRemainingSeconds = displayedRemainingSeconds;
            _timeLabel.SetDuration(TimeSpan.FromSeconds(displayedRemainingSeconds));
        }

        private float GetRemainingSeconds()
        {
            return Mathf.Max(0f, (float)_promotionOrder.GetRemainingTime().TotalSeconds);
        }

        private enum OrderState : byte
        {
            Available = 0,
            Active = 1,
            Completed = 2
        }
    }
}