using System;
using System.Linq;
using LL.User.Core.Progress;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User.Configuration
{
    [CreateAssetMenu(fileName = nameof(LevelProgressionConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class LevelProgressionConfig : ScriptableObject, ILevelProgressionSource
    {
        internal const string CreationPath = "LL/User/Level Progression Config";

        [InfoBox("Total accumulated experience required to reach each level.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private LevelThresholdEntry[] _levelThresholds;

        public LevelProgressionData Load() => new
        (
            _levelThresholds?.Select(threshold => threshold?.TotalExperience ?? 0)
        );

        private void OnValidate()
        {
            if (_levelThresholds == null || _levelThresholds.Length == 0)
            {
                _levelThresholds = new[] { new LevelThresholdEntry(1, 0) };
                return;
            }

            var previousExperience = 0;

            for (var index = 0; index < _levelThresholds.Length; index++)
            {
                var level = index + 1;
                var minimumExperience = index == 0
                    ? 0
                    : previousExperience + 1;

                var threshold = LevelThresholdEntry.CreateOrNormalize(
                    _levelThresholds[index],
                    level,
                    minimumExperience);

                _levelThresholds[index] = threshold;
                previousExperience = threshold.TotalExperience;
            }
        }
    }

    [Serializable]
    internal sealed class LevelThresholdEntry
    {
        [ReadOnly]
        [LabelText("Level")]
        [TableColumnWidth(60, Resizable = false)]
        [SerializeField] private int _level;

        [LabelText("Total Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _totalExperience;

        internal int TotalExperience => _totalExperience;

        internal LevelThresholdEntry(int level, int totalExperience)
        {
            _level = level;
            _totalExperience = totalExperience;
        }

        internal static LevelThresholdEntry CreateOrNormalize(
            LevelThresholdEntry threshold,
            int level,
            int minimumExperience)
        {
            threshold ??= new LevelThresholdEntry(level, minimumExperience);

            threshold._level = level;
            threshold._totalExperience = level == 1
                ? 0
                : Math.Max(threshold._totalExperience, minimumExperience);

            return threshold;
        }
    }
}