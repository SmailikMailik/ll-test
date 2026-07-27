using LL.UI.Typography;
using TMPro;
using UnityEngine;

namespace LL.UI.Localization
{
    [AddComponentMenu("LL/UI/Localization/Localized Sprite Label")]
    internal sealed class LocalizedSpriteLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private TextSprite _sprite;

        public void SetText(string localizedText)
        {
            var sprite = TextTags.Sprite(_sprite);
            var separator = TextSymbols.GetValue(TextSymbol.NonBreakingSpace);
            _label.text = $"{sprite}{separator}{localizedText}";
        }
    }
}