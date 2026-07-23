using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace LL.UI
{
    internal sealed class GlobalMessage : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private TMP_Text _messageLabel;
        [SerializeField] private Vector2 _startPosition;
        [SerializeField] private Vector2 _finishPosition;
        [SerializeField] private float _moveSeconds = 0.3f;

        private readonly Queue<MessageParams> _showQueue = new();
        private Coroutine _showRoutine;

        private void Start()
        {
            ForceMoveTo(_startPosition);
        }

        /// <summary> Отобразить сообщение мгновенно. </summary>
        /// <param name="text"> Текст сообщения. </param>
        internal void ShowForced(string text) => Show(text, false);

        /// <summary> Отобразить сообщение в порядке очереди. </summary>
        /// <param name="text"> Текст сообщения. </param>
        internal void ShowQueued(string text) => Show(text, true);

        private void Show(string text, bool addToQueue)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            Show(new MessageParams(text), addToQueue);
        }

        private void Show(MessageParams messageParams, bool addToQueue)
        {
            if (_showRoutine is not null)
            {
                if (addToQueue)
                {
                    _showQueue.Enqueue(messageParams);
                    return;
                }

                StopCoroutine(_showRoutine);
                _showRoutine = null;
            }

            _showRoutine = StartCoroutine(Show(messageParams));
        }

        private IEnumerator Show(MessageParams messageParams)
        {
            _messageLabel.text = messageParams.Text;
            ForceMoveTo(_startPosition);

            yield return SmoothMoveTo(_finishPosition);
            yield return new WaitForSecondsRealtime(messageParams.DisplaySeconds);
            yield return SmoothMoveTo(_startPosition);

            _showRoutine = null;

            if (_showQueue.Count != 0)
                Show(_showQueue.Dequeue(), false);
        }

        private IEnumerator SmoothMoveTo(Vector2 position)
        {
            yield return _container.DOAnchorPos(position, _moveSeconds).SetUpdate(true).Play().WaitForCompletion();
        }

        private void ForceMoveTo(Vector2 position)
        {
            _container.anchoredPosition = position;
        }

        #region DEV

        [ContextMenu("DEV/ShowForced/NullMessage")]
        private void DEV_ShowForced_NullMessage() => ShowForced(
            null);

        [ContextMenu("DEV/ShowForced/EmptyMessage")]
        private void DEV_ShowForced_EmptyMessage() => ShowForced(
            string.Empty);

        [ContextMenu("DEV/ShowForced/SmallMessage")]
        private void DEV_ShowForced_SmallMessage() => ShowForced(
            "Hello! It's forced small message.");

        [ContextMenu("DEV/ShowForced/LargeMessage")]
        private void DEV_ShowForced_LargeMessage() => ShowForced(
            "Hello! It's forced large message. More characters need to be displayed so this message won't make any sense.");

        [ContextMenu("DEV/ShowQueued/NullMessage")]
        private void DEV_ShowQueued_NullMessage() => ShowQueued(
            null);

        [ContextMenu("DEV/ShowQueued/EmptyMessage")]
        private void DEV_ShowQueued_EmptyMessage() => ShowQueued(
            string.Empty);

        [ContextMenu("DEV/ShowQueued/SmallMessage")]
        private void DEV_ShowQueued_SmallMessage() => ShowQueued(
            "Hello! It's queued small message.");

        [ContextMenu("DEV/ShowQueued/LargeMessage")]
        private void DEV_ShowQueued_LargeMessage() => ShowQueued(
            "Hello! It's queued large message. More characters need to be displayed so this message won't make any sense.");

        #endregion

        private sealed class MessageParams
        {
            private const float MinDisplaySeconds = 1f;
            private const float MaxDisplaySeconds = 5f;
            private const float SecondsPerCharacter = 0.1f; // 2 second for 20 characters

            internal string Text { get; }
            internal float DisplaySeconds { get; }

            internal MessageParams(string text)
            {
                Text = text;
                DisplaySeconds = GetDisplaySeconds(text);
            }

            private float GetDisplaySeconds(string text)
            {
                var displaySeconds = text.Length * SecondsPerCharacter;
                return Mathf.Clamp(displaySeconds, MinDisplaySeconds, MaxDisplaySeconds);
            }
        }
    }
}