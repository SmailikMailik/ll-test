using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace LL.UI.Bar.Marked
{
    internal sealed class MarkedBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private MarkedBarItem _itemTemplate;

        private readonly List<MarkedBarItem> _activeItems = new();

        private IObjectPool<MarkedBarItem> _itemsPool;

        internal void Awake()
        {
            _itemTemplate.gameObject.SetActive(false);

            _itemsPool = new ObjectPool<MarkedBarItem>(
                () => Instantiate(_itemTemplate, _container),
                item => item.gameObject.SetActive(true),
                item => item.gameObject.SetActive(false),
                item => Destroy(item.gameObject));
        }

        internal void SetValue(int value)
        {
            foreach (var item in _activeItems)
                _itemsPool.Release(item);

            _activeItems.Clear();

            for (var i = 0; i < value; i++)
            {
                var item = _itemsPool.Get();
                item.Enable();

                if (i == value - 1)
                    item.Disable();

                _activeItems.Add(item);
            }
        }
    }
}