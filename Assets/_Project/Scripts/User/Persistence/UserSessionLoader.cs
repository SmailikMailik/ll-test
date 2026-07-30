using System;
using LL.Infrastructure.Loading;
using LL.User.Configuration;
using LL.User.Snapshots;
using UnityEngine;
using VContainer;

namespace LL.User.Persistence
{
    internal sealed class UserSessionLoader : IDataLoader<UserSnapshot>
    {
        private readonly IUserDefaultsFactory _defaultsFactory;
        private readonly IUserSaveRepository _repository;

        [Inject]
        internal UserSessionLoader(
            IUserDefaultsFactory defaultsFactory,
            IUserSaveRepository repository)
        {
            _defaultsFactory = defaultsFactory ?? throw new ArgumentNullException(nameof(defaultsFactory));
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

            var defaultSnapshot = _defaultsFactory.CreateSnapshot();
            _repository.Save(defaultSnapshot);
            return defaultSnapshot;
        }
    }
}