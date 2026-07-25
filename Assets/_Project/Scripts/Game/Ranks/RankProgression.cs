using System;
using System.Collections.Generic;
using VContainer;

namespace LL.Game.Ranks
{
    internal sealed class RankProgression : IRankProgression
    {
        private readonly IReadOnlyList<int> _experienceThresholds;

        [Inject]
        internal RankProgression(IDataLoader<RankCatalog> loader)
        {
            if (loader is null)
                throw new ArgumentNullException(nameof(loader));

            _experienceThresholds = Normalize(loader.Load()?.ExperienceThresholds);
        }

        public int GetRank(int totalExperience)
        {
            var experience = Math.Max(0, totalExperience);

            for (var index = 1; index < _experienceThresholds.Count; index++)
            {
                if (experience < _experienceThresholds[index])
                    return index;
            }

            return _experienceThresholds.Count;
        }

        private static IReadOnlyList<int> Normalize(IReadOnlyList<int> thresholds)
        {
            if (thresholds == null || thresholds.Count == 0)
                return new[] { 0 };

            var normalized = new int[thresholds.Count];

            for (var index = 1; index < thresholds.Count; index++)
            {
                normalized[index] = Math.Max(
                    thresholds[index],
                    normalized[index - 1] + 1);
            }

            return normalized;
        }
    }
}