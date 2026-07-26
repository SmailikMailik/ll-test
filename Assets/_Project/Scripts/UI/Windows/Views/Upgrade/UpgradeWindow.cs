using LL.Extensions;
using LL.Game.Ranks;
using LL.User.Core.Progress;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views.Upgrade
{
    internal sealed class UpgradeWindow : Window<UpgradeWindowParameters>
    {
        [SerializeField] private PredictedProgressBar _bar;
        [SerializeField] private TMP_Text _rankLabel;
        [SerializeField] private TMP_Text _expLabel;
        [SerializeField] private TMP_Text _addLabel;

        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;

        private int _appliedExperience;
        private int _pendingExperience;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            IRankProgression rankProgression)
        {
            _userProgress = userProgress;
            _rankProgression = rankProgression;
        }

        protected override void OnShow()
        {
            _appliedExperience = _userProgress.CurrentTotalExperience;
            _pendingExperience = 0;

            ShowProgress();
        }

        protected override void OnHide()
        {
            _pendingExperience = 0;
        }

        private void ShowProgress()
        {
            var previewExperience = _appliedExperience + _pendingExperience;
            var progress = _rankProgression.GetProgress(previewExperience);

            _rankLabel.text = progress.Rank.ToString();
            _expLabel.text = progress.HasNextRank
                ? $"{progress.TotalExperience.ToNumber()}/{progress.NextRankExperience.ToNumber()}"
                : progress.TotalExperience.ToNumber();
            _addLabel.text = _pendingExperience > 0
                ? $"+ {_pendingExperience.ToNumber()}"
                : string.Empty;

            var currentProgress = progress.HasNextRank
                ? Mathf.Clamp01(
                    (float)(_appliedExperience - progress.CurrentRankExperience) /
                    progress.ExperienceBetweenRanks)
                : 1f;

            _bar.SetProgress(currentProgress, progress.NormalizedExperience);
        }

        private void AddPendingExperience(int amount)
        {
            var pendingExperience = (long)_pendingExperience + amount;
            var previewExperience = _appliedExperience + pendingExperience;

            if (amount <= 0 || previewExperience > int.MaxValue)
                return;

            _pendingExperience = (int)pendingExperience;
            ShowProgress();
        }

        private void ApplyExperience()
        {
            if (_pendingExperience <= 0 ||
                _userProgress.TryAddExperience(_pendingExperience) is false)
                return;

            _appliedExperience = _userProgress.CurrentTotalExperience;
            _pendingExperience = 0;
            ShowProgress();
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private void OnGUI()
        {
            if (IsVisible is false)
                return;

            GUILayout.BeginArea(
                new Rect(16f, Mathf.Max(0f, Screen.height - 164f), 120f, 148f),
                GUI.skin.box);

            if (GUILayout.Button("+100 XP"))
                AddPendingExperience(100);

            if (GUILayout.Button("+500 XP"))
                AddPendingExperience(500);

            if (GUILayout.Button("+2000 XP"))
                AddPendingExperience(2000);

            var guiEnabled = GUI.enabled;
            GUI.enabled = _pendingExperience > 0;

            if (GUILayout.Button("Apply"))
                ApplyExperience();

            GUI.enabled = guiEnabled;

            GUILayout.EndArea();
        }
#endif
    }

    internal sealed class UpgradeWindowParameters : IWindowParameters
    {
        internal int Id { get; }

        internal UpgradeWindowParameters(int id)
        {
            Id = id;
        }
    }
}