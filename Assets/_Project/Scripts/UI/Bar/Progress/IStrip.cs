using System.Collections;
using LL.Coroutines;
using LL.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Bar.Progress
{
    internal enum StripType : byte
    {
        Common = 0,
        Shadow = 1
    }

    internal interface IStrip
    {
        void SetFillAmount(float value, float maxValue);
    }

    internal sealed class StripCommon : IStrip
    {
        private readonly Image _foregroundImage;

        internal StripCommon(Image foregroundImage)
        {
            _foregroundImage = foregroundImage;
        }

        public void SetFillAmount(float value, float maxValue)
        {
            var normalizedValue = value.Normalize(maxValue);
            _foregroundImage.fillAmount = normalizedValue;
        }
    }

    internal sealed class StripShadow : IStrip
    {
        private readonly MonoBehaviour _owner;
        private readonly Image _foregroundImage;
        private readonly Image _shadowImage;

        private Coroutine _movingRoutine;

        internal StripShadow(MonoBehaviour owner, Image foregroundImage, Image shadowImage)
        {
            _owner = owner;
            _foregroundImage = foregroundImage;
            _shadowImage = shadowImage;
        }

        public void SetFillAmount(float value, float maxValue)
        {
            var normalizedValue = value.Normalize(maxValue);

            if (_foregroundImage.fillAmount > normalizedValue)
                MoveShadow(normalizedValue);
            else
                _shadowImage.fillAmount = normalizedValue;

            _foregroundImage.fillAmount = normalizedValue;
        }

        private void MoveShadow(float newFillAmount)
        {
            if (_movingRoutine != null)
            {
                _owner.StopCoroutine(_movingRoutine);
                _movingRoutine = null;
            }

            _movingRoutine = _owner.StartCoroutine(MoveShadowRoutine(newFillAmount));
        }

        private IEnumerator MoveShadowRoutine(float newFillAmount)
        {
            yield return Yielders.WaitForSeconds(0.5f);

            while (_shadowImage.fillAmount > newFillAmount)
            {
                _shadowImage.fillAmount -= 0.02f;
                yield return Yielders.WaitForSeconds(0.01f);
            }

            _shadowImage.fillAmount = newFillAmount;
            _movingRoutine = null;
        }
    }
}