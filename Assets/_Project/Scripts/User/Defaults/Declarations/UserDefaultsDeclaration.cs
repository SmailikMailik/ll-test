using System;
using System.Collections.Generic;
using LL.Infrastructure.Collections;

namespace LL.User.Defaults.Declarations
{
    internal sealed class UserDefaultsDeclaration
    {
        internal UserIdentityDefaultDeclaration Identity { get; }
        internal UserProgressDefaultDeclaration Progress { get; }
        internal IReadOnlyList<UserItemDefaultDeclaration> Items { get; }

        internal UserDefaultsDeclaration(
            UserIdentityDefaultDeclaration identity,
            UserProgressDefaultDeclaration progress,
            IEnumerable<UserItemDefaultDeclaration> items)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            Items = items.ToReadOnlyCopy();
        }
    }
}