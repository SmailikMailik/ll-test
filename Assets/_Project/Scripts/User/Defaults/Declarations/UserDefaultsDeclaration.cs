using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.User.Defaults.Declarations
{
    internal sealed class UserDefaultsDeclaration
    {
        internal string UserId { get; }
        internal string RegionCode { get; }
        internal string RankId { get; }
        internal int Experience { get; }
        internal IReadOnlyList<UserItemDefaultsDeclaration> Items { get; }

        internal UserDefaultsDeclaration(
            string userId,
            string regionCode,
            string rankId,
            int experience,
            IEnumerable<UserItemDefaultsDeclaration> items)
        {
            UserId = userId;
            RegionCode = regionCode;
            RankId = rankId;
            Experience = experience;
            Items = Array.AsReadOnly(items?.ToArray() ?? Array.Empty<UserItemDefaultsDeclaration>());
        }
    }
}