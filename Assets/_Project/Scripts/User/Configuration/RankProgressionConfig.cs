using System;
using System.Linq;
using LL.User.Core.Progress;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User.Configuration
{
    [CreateAssetMenu(fileName = nameof(RankProgressionConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class RankProgressionConfig : ScriptableObject, IRankProgressionSource
    {
        internal const string CreationPath = "LL/User/Rank Progression Config";

        [InfoBox("Total accumulated experience required to reach each rank.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private RankThresholdEntry[] _rankThresholds;

        public RankProgressionData Load() => new
        (
            _rankThresholds?.Select(threshold => threshold?.TotalExperience ?? 0)
        );

        private void OnValidate()
        {
            if (_rankThresholds == null || _rankThresholds.Length == 0)
            {
                _rankThresholds = new[] { new RankThresholdEntry(1, 0) };
                return;
            }

            var previousExperience = 0;

            for (var index = 0; index < _rankThresholds.Length; index++)
            {
                var rank = index + 1;
                var minimumExperience = index == 0
                    ? 0
                    : previousExperience + 1;

                var threshold = RankThresholdEntry.CreateOrNormalize(
                    _rankThresholds[index],
                    rank,
                    minimumExperience);

                _rankThresholds[index] = threshold;
                previousExperience = threshold.TotalExperience;
            }
        }
    }

    [Serializable]
    internal sealed class RankThresholdEntry
    {
        [ReadOnly]
        [LabelText("Rank")]
        [TableColumnWidth(60, Resizable = false)]
        [SerializeField] private int _rank;

        [LabelText("Total Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _totalExperience;

        internal int TotalExperience => _totalExperience;

        internal RankThresholdEntry(int rank, int totalExperience)
        {
            _rank = rank;
            _totalExperience = totalExperience;
        }

        internal static RankThresholdEntry CreateOrNormalize(
            RankThresholdEntry threshold,
            int rank,
            int minimumExperience)
        {
            threshold ??= new RankThresholdEntry(rank, minimumExperience);

            threshold._rank = rank;
            threshold._totalExperience = rank == 1
                ? 0
                : Math.Max(threshold._totalExperience, minimumExperience);

            return threshold;
        }
    }
}