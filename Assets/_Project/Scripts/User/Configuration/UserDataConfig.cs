using LL.User.Core.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User.Configuration
{
    [CreateAssetMenu(fileName = nameof(UserDataConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class UserDataConfig : ScriptableObject, IUserDataSource
    {
        internal const string CreationPath = "LL/User/User Data Config";

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

        UserData IUserDataSource.Load() => new
        (
            _regionCode,
            _userId,
            _softAmount,
            _hardAmount,
            _masterPointAmount,
            _totalExperience
        );
    }
}