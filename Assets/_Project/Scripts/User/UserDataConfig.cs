using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User
{
    [CreateAssetMenu(fileName = nameof(UserDataConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class UserDataConfig : ScriptableObject, IUserDataSource
    {
        internal const string CreationPath = "LL/User/User Data Config";

        [BoxGroup("Wallet")]
        [LabelText("Soft Currency")]
        [SuffixLabel("SOFT", true)]
        [MinValue(0)]
        [SerializeField] private int _softAmount = 99;

        [BoxGroup("Wallet")]
        [LabelText("Hard Currency")]
        [SuffixLabel("HARD", true)]
        [MinValue(0)]
        [SerializeField] private int _hardAmount = 99;

        [BoxGroup("Wallet")]
        [LabelText("Master Points")]
        [SuffixLabel("MP", true)]
        [MinValue(0)]
        [SerializeField] private int _masterPointAmount = 99;

        [BoxGroup("Progress")]
        [LabelText("Total Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _totalExperience = 12_861;

        UserDataSnapshot IUserDataSource.Load() => new
        (
            _softAmount,
            _hardAmount,
            _masterPointAmount,
            _totalExperience
        );
    }
}