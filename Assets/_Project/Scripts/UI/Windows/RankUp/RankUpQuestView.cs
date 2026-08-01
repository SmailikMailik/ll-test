using System;
using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.Ranks;
using LL.Game.RankUp;
using LL.Game.RankUp.Services;
using LL.Game.Quests;
using LL.Presentation.Localization;
using LL.Presentation.Quests;
using LL.Presentation.RankUp;
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
        private HeroCatalog _heroes;
        private ILocalizationService _localization;
        private IUserRankUpAttempts _attempts;
        private IRankUpQuestRequirementService _questService;

        private QuestState _state = QuestState.Available;
        private HeroId _heroId;
        private RankId _rankId;
        private RankUpOptionId _optionId;
        private QuestRankUpRequirementDefinition _definition;
        private QuestDefinition _quest;
        private int _displayedRemainingSeconds = -1;
        private bool _canAccept;

        [Inject]
        private void Construct(
            IQuestCompletionConfirmation completionConfirmation,
            HeroCatalog heroes,
            ILocalizationService localization,
            IUserRankUpAttempts attempts,
            IRankUpQuestRequirementService questService)
        {
            _completionConfirmation = completionConfirmation ?? throw new ArgumentNullException(nameof(completionConfirmation));
            _heroes = heroes ?? throw new ArgumentNullException(nameof(heroes));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _attempts = attempts ?? throw new ArgumentNullException(nameof(attempts));
            _questService = questService ?? throw new ArgumentNullException(nameof(questService));
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
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            QuestRankUpRequirementDefinition definition,
            QuestDefinition quest,
            bool canAccept)
        {
            _heroId = heroId;
            _rankId = rankId;
            _optionId = optionId;
            _definition = definition ?? throw new ArgumentNullException(nameof(definition));
            _quest = quest ?? throw new ArgumentNullException(nameof(quest));
            _canAccept = canAccept;
            RestoreState();
            RefreshText();
            RefreshState();
        }

        internal void SetAvailable(bool canAccept)
        {
            _canAccept = canAccept;
            RefreshButtonAvailability();
        }

        internal void ClearQuest()
        {
            _questService.Clear(_heroId, _rankId, _optionId);
            SetState(QuestState.Available);
        }

        private void RefreshText()
        {
            var hero = _heroes.GetHero(_heroId);
            var unlockText = TextTags.Style(
                _localization.GetText(RankUpLocalizationKeys.Unlock),
                TextStyle.Accent);
            var countText = TextTags.Style(
                TextFormatter.Number(_definition.RequiredCount),
                TextStyle.Accent);

            _titleLabel.text = _localization.GetText(
                _quest.TitleLocalizationKey,
                new Dictionary<string, object>
                {
                    [UnlockVariable] = unlockText
                });
            _descriptionLabel.text = _localization.GetText(
                _quest.DescriptionLocalizationKey,
                new Dictionary<string, object>
                {
                    [CountVariable] = countText,
                    [HeroVariable] = _localization.GetText(hero.NameLocalizationKey)
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
            if (_questService.TryStart(_heroId, _rankId, _optionId, _definition) is false)
                return false;

            SetState(QuestState.Active);
            return true;
        }

        private void OnQuestCompletionConfirmed()
        {
            if (_state != QuestState.Active ||
                _questService.TryCompleteForTesting(_heroId, _rankId, _optionId, _definition) is false)
            {
                return;
            }

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
            _questService.TryExpire(_heroId, _rankId, _optionId, _definition);

            if (_attempts.TryGetQuest(_heroId, _rankId, _optionId, _definition.Id, out var state) is false)
            {
                _state = QuestState.Available;
                return;
            }

            _state = state.CurrentCount >= _definition.RequiredCount
                ? QuestState.Completed
                : QuestState.Active;
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
            if (_definition is not null && _quest is not null)
                RefreshText();

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
                QuestState.Available => Mathf.Max(0f, (float)_definition.Duration.TotalSeconds),
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
            if (_attempts.TryGetQuest(_heroId, _rankId, _optionId, _definition.Id, out var state) is false)
                return 0f;

            return Mathf.Max(0f, (float)_attempts.GetRemainingTime(state).TotalSeconds);
        }

        private enum QuestState : byte
        {
            Available = 0,
            Active = 1,
            Completed = 2
        }
    }
}