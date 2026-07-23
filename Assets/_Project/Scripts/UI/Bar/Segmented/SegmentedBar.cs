using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace LL.UI.Bar.Segmented
{
    internal sealed class SegmentedBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private SegmentedBarItem _itemTemplate;

        private readonly List<SegmentedBarItem> _activeItems = new();

        private IObjectPool<SegmentedBarItem> _itemsPool;

        internal void Awake()
        {
            _itemTemplate.gameObject.SetActive(false);

            _itemsPool = new ObjectPool<SegmentedBarItem>(
                () => Instantiate(_itemTemplate, _container),
                item => item.gameObject.SetActive(true),
                item => item.gameObject.SetActive(false),
                item => Destroy(item.gameObject));
        }

        internal void SetValue(int value, int maxValue)
        {
            value = Mathf.Clamp(value, 0, maxValue);

            foreach (var item in _activeItems)
                _itemsPool.Release(item);

            _activeItems.Clear();

            for (var i = 0; i < maxValue; i++)
            {
                var item = _itemsPool.Get();
                item.transform.SetAsLastSibling();

                if (i < value)
                    item.Enable();
                else
                    item.Disable();

                _activeItems.Add(item);
            }
        }
    }
}