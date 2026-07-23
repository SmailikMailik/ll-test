using TMPro;
using UnityEngine;

namespace LL.UI
{
    internal sealed class CurrencyItemView : MonoBehaviour
    {
        [field: SerializeField] internal TMP_Text Label { get; private set; }
        [field: SerializeField] internal CommonButton Button { get; private set; }
    }
}