using System;
using LL.Infrastructure.Loading;
using LL.User.Defaults;
using LL.User.Snapshots;
using UnityEngine;
using VContainer;

namespace LL.User.Persistence
{
    internal sealed class UserSessionLoader : IDataLoader<UserSnapshot>
    {
        private readonly UserDefaultsTemplate _defaults;
        private readonly IUserSaveRepository _repository;

        [Inject]
        internal UserSessionLoader(
            UserDefaultsTemplate defaults,
            IUserSaveRepository repository)
        {
            _defaults = defaults ?? throw new ArgumentNullException(nameof(defaults));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public UserSnapshot Load()
        {
            var result = _repository.Load();

            if (result.Status == UserLoadStatus.Loaded)
                return result.Snapshot;

            if (result.Status == UserLoadStatus.UnsupportedVersion)
            {
                Debug.LogWarning($"Unsupported user document version {result.DocumentVersion}. Resetting user data.");
            }
            else if (result.Status == UserLoadStatus.Corrupted)
            {
                Debug.LogWarning("User document is corrupted. Resetting user data.");
            }

            var defaultSnapshot = _defaults.CreateSnapshot();
            _repository.Save(defaultSnapshot);
            return defaultSnapshot;
        }
    }
}