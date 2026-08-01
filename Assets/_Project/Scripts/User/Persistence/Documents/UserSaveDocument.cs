using System;

namespace LL.User.Persistence.Documents
{
    internal sealed class UserSaveDocument
    {
        internal const int CurrentVersion = 2;

        public int Version { get; }
        public UserIdentityDocumentEntry Identity { get; }
        public UserHeroSelectionDocumentEntry HeroSelection { get; }
        public UserHeroDocumentEntry[] Heroes { get; }
        public UserItemDocumentEntry[] Items { get; }

        public UserSaveDocument(
            int version,
            UserIdentityDocumentEntry identity,
            UserHeroSelectionDocumentEntry heroSelection,
            UserHeroDocumentEntry[] heroes,
            UserItemDocumentEntry[] items)
        {
            Version = version;
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            HeroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
            Heroes = heroes ?? Array.Empty<UserHeroDocumentEntry>();
            Items = items ?? Array.Empty<UserItemDocumentEntry>();
        }
    }
}