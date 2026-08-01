using System;
using System.Collections.Generic;
using LL.Infrastructure.Collections;

namespace LL.User.Defaults.Declarations
{
    internal sealed class UserDefaultsDeclaration
    {
        internal UserIdentityDefaultDeclaration Identity { get; }
        internal UserHeroSelectionDefaultDeclaration HeroSelection { get; }
        internal UserProgressDefaultDeclaration Progress { get; }
        internal IReadOnlyList<UserItemDefaultDeclaration> Items { get; }

        internal UserDefaultsDeclaration(
            UserIdentityDefaultDeclaration identity,
            UserHeroSelectionDefaultDeclaration heroSelection,
            UserProgressDefaultDeclaration progress,
            IEnumerable<UserItemDefaultDeclaration> items)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            HeroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            Items = items.ToReadOnlyCopy();
        }
    }
}