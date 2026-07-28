using LL.UI.Typography;
using TMPro;
using UnityEngine;

namespace LL.UI
{
    [DisallowMultipleComponent]
    [AddComponentMenu("LL/UI/Inline Sprite Label")]
    internal sealed class InlineSpriteLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private TextSprite _sprite;
        [SerializeField] private InlineSpritePlacement _placement;

        private enum InlineSpritePlacement : byte
        {
            BeforeText = 0,
            AfterText = 1
        }

        public void OnTextChanged(string text)
        {
            var sprite = TextTags.Sprite(_sprite);
            var separator = TextSymbols.GetValue(TextSymbol.NonBreakingSpace);
            _label.text = _placement switch
            {
                InlineSpritePlacement.BeforeText => $"{sprite}{separator}{text}",
                InlineSpritePlacement.AfterText => $"{text}{separator}{sprite}",
                _ => text
            };
        }
    }
}