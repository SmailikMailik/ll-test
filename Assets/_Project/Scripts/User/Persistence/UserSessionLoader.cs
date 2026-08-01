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
        private readonly UserDefaultsSnapshot _defaults;
        private readonly IUserSaveRepository _repository;
        private readonly UserSnapshotReconciler _reconciler;

        [Inject]
        internal UserSessionLoader(
            UserDefaultsSnapshot defaults,
            IUserSaveRepository repository,
            UserSnapshotReconciler reconciler)
        {
            _defaults = defaults ?? throw new ArgumentNullException(nameof(defaults));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _reconciler = reconciler ?? throw new ArgumentNullException(nameof(reconciler));
        }

        public UserSnapshot Load()
        {
            var result = _repository.Load();

            if (result.Status == UserLoadStatus.Loaded)
            {
                var reconciliation = _reconciler.Reconcile(result.Snapshot);

                if (reconciliation.Status != UserReconciliationStatus.Incompatible)
                {
                    if (reconciliation.Status == UserReconciliationStatus.Changed &&
                        _repository.Save(reconciliation.Snapshot) is false)
                    {
                        Debug.LogError("Reconciled user data could not be saved.");
                    }

                    return reconciliation.Snapshot;
                }

                Debug.LogWarning("User data is incompatible with current game data. Resetting user data.");
            }

            if (result.Status == UserLoadStatus.UnsupportedVersion)
            {
                Debug.LogWarning($"Unsupported user document version {result.DocumentVersion}. Resetting user data.");
            }
            else if (result.Status == UserLoadStatus.Corrupted)
            {
                Debug.LogWarning("User document is corrupted. Resetting user data.");
            }

            var defaultSnapshot = _defaults.CreateUserSnapshot();

            if (_repository.Save(defaultSnapshot) is false)
                Debug.LogError("Default user data could not be saved.");

            return defaultSnapshot;
        }
    }
}