using UnityEngine;

namespace LL.UI
{
    internal sealed class ChaoticMovement : RectMonoBehaviour
    {
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _intensity = 10f;
        [SerializeField] private float _changeInterval = 0.1f;

        private Vector2 _randomDirection;
        private float _timeSinceChange;

        private void Start()
        {
            SetRandomDirection();
        }

        private void Update()
        {
            _timeSinceChange += Time.deltaTime;

            // Изменяем направление каждые changeInterval секунд
            if (_timeSinceChange >= _changeInterval)
            {
                SetRandomDirection();
                _timeSinceChange = 0f;
            }

            // Изменяем позицию на основе выбранного случайного направления
            RectTransform.anchoredPosition += _randomDirection * _speed * Time.deltaTime;
        }

        private void SetRandomDirection()
        {
            // Генерируем случайное направление в пределах [-1, 1] по обеим осям
            var randomX = Random.Range(-1f, 1f);
            var randomY = Random.Range(-1f, 1f);

            // Нормализуем направление и умножаем на интенсивность
            _randomDirection = new Vector2(randomX, randomY).normalized * _intensity;
        }
    }
}