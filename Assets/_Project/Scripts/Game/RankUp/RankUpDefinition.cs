using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Identifiers;
using LL.Game.Ranks;
using LL.Game.Rewards;

namespace LL.Game.RankUp
{
    internal sealed class RankUpDefinition
    {
        internal HeroId HeroId { get; }
        internal RankId RankId { get; }
        internal IReadOnlyList<RankUpOptionDefinition> Options { get; }
        internal RewardId RewardId { get; }

        private readonly IReadOnlyDictionary<RankUpOptionId, RankUpOptionDefinition> _optionsById;

        internal RankUpDefinition(
            HeroId heroId,
            RankId rankId,
            IEnumerable<RankUpOptionDefinition> options,
            RewardId rewardId)
        {
            IdentifierValidator.EnsureValid(heroId, nameof(heroId));
            IdentifierValidator.EnsureValid(rankId, nameof(rankId));
            IdentifierValidator.EnsureValid(rewardId, nameof(rewardId));

            var optionArray = options?.ToArray() ?? Array.Empty<RankUpOptionDefinition>();

            if (optionArray.Length == 0)
                throw new ArgumentException("Rank-up must contain at least one option.", nameof(options));

            IdentifierCollectionValidator.EnsureValid(
                optionArray,
                option => option.Id,
                nameof(options));

            HeroId = heroId;
            RankId = rankId;
            Options = Array.AsReadOnly(optionArray);
            RewardId = rewardId;
            _optionsById = optionArray.ToDictionary(option => option.Id);
        }

        internal bool TryGetOption(RankUpOptionId optionId, out RankUpOptionDefinition option) =>
            _optionsById.TryGetValue(optionId, out option);
    }
}