using System;
using System.Collections.Generic;
using LL.Game.RankUp;
using LL.Game.Quests;
using LL.Presentation.Localization;
using LL.Presentation.RankUp;
using LL.Presentation.Quests;
using LL.Presentation.Typography;
using LL.UI.Controls;
using LL.UI.Localization;
using LL.User.State.RankUp;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.RankUp
{
    [DisallowMultipleComponent]
    internal sealed class RankUpQuestView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _descriptionLabel;
        [SerializeField] private LocalizedDurationLabel _timeLabel;
        [SerializeField] private TMP_Text _buttonLabel;
        [SerializeField] private InteractiveButton _button;

        private const string CountVariable = "count";
        private const string HeroVariable = "hero";
        private const string UnlockVariable = "unlock";

        private static readonly TimeSpan _timerTickInterval = TimeSpan.FromSeconds(1);

        internal Observable<Unit> Completed => _completed;
        internal bool IsCompleted => _state == QuestState.Completed;

        private readonly Subject<Unit> _completed = new();

        private IQuestCompletionConfirmation _completionConfirmation;
        private ILocalizationService _localization;
        private IUserRankUpQuest _rankUpQuest;

        private QuestState _state = QuestState.Available;
        private RankUpQuest _rankUpQuestDefinition;
        private QuestDefinition _quest;
        private int _displayedRemainingSeconds = -1;
        private bool _canAccept;
        private TimeSpan _duration;

        [Inject]
        private void Construct(
            IQuestCompletionConfirmation completionConfirmation,
            ILocalizationService localization,
            IUserRankUpQuest rankUpQuest)
        {
            _completionConfirmation = completionConfirmation
                ?? throw new ArgumentNullException(nameof(completionConfirmation));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _rankUpQuest = rankUpQuest ?? throw new ArgumentNullException(nameof(rankUpQuest));
        }

        private void Start()
        {
            _completed.AddTo(this);
            _button.Clicked.Subscribe(_ => OnButtonClicked()).AddTo(this);
            _localization.LocaleChanged.Subscribe(_ => OnLocaleChanged()).AddTo(this);

            Observable
                .Interval(_timerTickInterval)
                .Where(_ => _state == QuestState.Active)
                .Subscribe(_ => TickTimer())
                .AddTo(this);
        }

        internal void Refresh(
            RankUpQuest rankUpQuest,
            QuestDefinition quest,
            bool canAccept)
        {
            _rankUpQuestDefinition = rankUpQuest ?? throw new ArgumentNullException(nameof(rankUpQuest));
            _quest = quest ?? throw new ArgumentNullException(nameof(quest));
            _duration = rankUpQuest.Duration;
            _canAccept = canAccept;
            RestoreState();
            RefreshText(rankUpQuest, quest);
            RefreshState();
        }

        internal void SetAvailable(bool canAccept)
        {
            _canAccept = canAccept;
            RefreshButtonAvailability();
        }

        internal void ClearQuest()
        {
            _rankUpQuest.ClearQuest();
            SetState(QuestState.Available);
        }

        private void RefreshText(
            RankUpQuest rankUpQuest,
            QuestDefinition quest)
        {
            var unlockText = TextTags.Style(
                _localization.GetText(RankUpLocalizationKeys.Unlock),
                TextStyle.Accent);
            var countText = TextTags.Style(
                TextFormatter.Number(rankUpQuest.RequiredAmount),
                TextStyle.Accent);

            _titleLabel.text = _localization.GetText(
                quest.TitleLocalizationKey,
                new Dictionary<string, object>
                {
                    [UnlockVariable] = unlockText
                });
            _descriptionLabel.text = _localization.GetText(
                quest.DescriptionLocalizationKey,
                new Dictionary<string, object>
                {
                    [CountVariable] = countText,
                    [HeroVariable] = _localization.GetText(rankUpQuest.HeroLocalizationKey)
                });
        }

        private void RefreshButtonText()
        {
            var localizationKey = _state switch
            {
                QuestState.Available => QuestLocalizationKeys.AcceptAction,
                QuestState.Active => QuestLocalizationKeys.CompleteAction,
                _ => QuestLocalizationKeys.CompletedLabel
            };

            _buttonLabel.text = _localization.GetText(localizationKey);
        }

        private void OnButtonClicked()
        {
            if (_canAccept is false || IsCompleted)
                return;

            if (_state == QuestState.Available && StartQuest() is false)
                return;

            _completionConfirmation.Confirm(OnQuestCompletionConfirmed);
        }

        private bool StartQuest()
        {
            if (_rankUpQuest.TryStart(_rankUpQuestDefinition.QuestId, _duration) is false)
                return false;

            SetState(QuestState.Active);
            return true;
        }

        private void OnQuestCompletionConfirmed()
        {
            if (_state != QuestState.Active || _rankUpQuest.TryComplete() is false)
                return;

            SetState(QuestState.Completed);
            _completed.OnNext(Unit.Default);
        }

        private void SetState(QuestState state)
        {
            _state = state;
            RefreshState();
        }

        private void RestoreState()
        {
            if (_rankUpQuest.QuestId.Equals(_rankUpQuestDefinition.QuestId) is false)
                _rankUpQuest.ClearQuest();

            _rankUpQuest.TryExpire();

            if (_rankUpQuest.IsCompleted)
                _state = QuestState.Completed;
            else if (_rankUpQuest.IsActive)
                _state = QuestState.Active;
            else
                _state = QuestState.Available;
        }

        private void RefreshState()
        {
            _displayedRemainingSeconds = -1;
            var showTime = _state != QuestState.Completed;
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
            if (_rankUpQuestDefinition != null && _quest != null)
                RefreshText(_rankUpQuestDefinition, _quest);

            RefreshButtonText();
        }

        private void TickTimer()
        {
            var remainingSeconds = GetRemainingSeconds();

            if (remainingSeconds <= 0f)
            {
                ClearQuest();
                return;
            }

            RefreshTime(remainingSeconds);
        }

        private void RefreshTime()
        {
            var remainingSeconds = _state switch
            {
                QuestState.Available => Mathf.Max(0f, (float)_duration.TotalSeconds),
                QuestState.Active => GetRemainingSeconds(),
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
            return Mathf.Max(0f, (float)_rankUpQuest.GetRemainingTime().TotalSeconds);
        }

        private enum QuestState : byte
        {
            Available = 0,
            Active = 1,
            Completed = 2
        }
    }
}