using LL.User.Core;
using LL.User.Core.Identity;
using LL.User.Core.Progress;
using LL.User.Core.Wallet;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User.Configuration
{
    [CreateAssetMenu(fileName = nameof(UserDefaultsConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class UserDefaultsConfig : ScriptableObject, IDefaultDataLoader<UserInitialData>
    {
        internal const string CreationPath = "LL/User/User Defaults Config";

        [BoxGroup("Identity")]
        [LabelText("Region Code")]
        [SerializeField] private string _regionCode;

        [BoxGroup("Identity")]
        [LabelText("User ID")]
        [SerializeField] private string _userId;

        [BoxGroup("Wallet")]
        [LabelText("Soft Currency")]
        [SuffixLabel("SOFT", true)]
        [MinValue(0)]
        [SerializeField] private int _softAmount;

        [BoxGroup("Wallet")]
        [LabelText("Hard Currency")]
        [SuffixLabel("HARD", true)]
        [MinValue(0)]
        [SerializeField] private int _hardAmount;

        [BoxGroup("Wallet")]
        [LabelText("Master Points")]
        [SuffixLabel("MP", true)]
        [MinValue(0)]
        [SerializeField] private int _masterPointAmount;

        [BoxGroup("Progress")]
        [LabelText("Total Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _totalExperience;

        UserInitialData IDataLoader<UserInitialData>.Load() => new
        (
            new UserIdentity(_regionCode, _userId),
            new WalletInitialData(_softAmount, _hardAmount, _masterPointAmount),
            new ProgressInitialData(_totalExperience)
        );
    }
}