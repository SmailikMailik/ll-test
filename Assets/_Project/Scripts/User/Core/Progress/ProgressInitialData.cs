using System;
using LL.Game.Ranks;

namespace LL.User.Core.Progress
{
    internal sealed class ProgressInitialData
    {
        internal int? SavedRank { get; }
        internal int TotalExperience { get; }

        internal ProgressInitialData(int totalExperience) : this(null, totalExperience) { }

        internal ProgressInitialData(int rank, int totalExperience)
            : this((int?)Math.Max(1, rank), totalExperience) { }

        private ProgressInitialData(int? savedRank, int totalExperience)
        {
            SavedRank = savedRank;
            TotalExperience = Math.Max(0, totalExperience);
        }

        internal int ResolveRank(IRankProgression rankProgression)
        {
            if (rankProgression == null)
                throw new ArgumentNullException(nameof(rankProgression));

            return SavedRank ?? rankProgression.GetRank(TotalExperience);
        }
    }
}