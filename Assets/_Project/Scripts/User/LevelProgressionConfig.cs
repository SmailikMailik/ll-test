using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User
{
    [CreateAssetMenu(fileName = nameof(LevelProgressionConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class LevelProgressionConfig : ScriptableObject, ILevelProgression
    {
        internal const string CreationPath = "LL/User/Level Progression Config";

        [InfoBox("Total accumulated experience required to reach each level.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private LevelThreshold[] _levelThresholds =
        {
            new(1, 0),
            new(2, 5000),
            new(3, 10000),
            new(4, 15000),
            new(5, 20000),
            new(6, 25000),
            new(7, 30000),
            new(8, 35000),
            new(9, 40000)
        };

        public int GetLevel(int totalExperience)
        {
            var experience = Math.Max(0, totalExperience);

            for (var index = 1; index < _levelThresholds.Length; index++)
            {
                if (experience < _levelThresholds[index].TotalExperience)
                    return index;
            }

            return Math.Max(1, _levelThresholds.Length);
        }

        private void OnValidate()
        {
            if (_levelThresholds == null || _levelThresholds.Length == 0)
            {
                _levelThresholds = new[] { new LevelThreshold(1, 0) };
                return;
            }

            var previousExperience = -1;

            for (var index = 0; index < _levelThresholds.Length; index++)
            {
                _levelThresholds[index] ??= new LevelThreshold();
                _levelThresholds[index].Validate(index + 1, previousExperience + 1);
                previousExperience = _levelThresholds[index].TotalExperience;
            }
        }
    }

    [Serializable]
    internal sealed class LevelThreshold
    {
        internal int TotalExperience => _totalExperience;

        [ReadOnly]
        [LabelText("Level")]
        [TableColumnWidth(60, Resizable = false)]
        [SerializeField] private int _level;

        [LabelText("Total Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _totalExperience;

        internal LevelThreshold() { }

        internal LevelThreshold(int level, int totalExperience)
        {
            _level = level;
            _totalExperience = totalExperience;
        }

        internal void Validate(int level, int minimumExperience)
        {
            _level = level;
            _totalExperience = Math.Max(_totalExperience, minimumExperience);
        }
    }
}