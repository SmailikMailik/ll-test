using System;
using System.Collections;
using UnityEngine;

namespace LL.Coroutines
{
    internal static class CoroutineExtensions
    {
        /// <summary> Вызов метода с задержкой в игровых секундах. </summary>
        internal static Coroutine WaitForSeconds(this MonoBehaviour @this, float seconds, Action callback) =>
            @this.StartCoroutine(Process(Yielders.WaitForSeconds(seconds), callback));

        /// <summary> Вызов метода с задержкой в реальных секундах. </summary>
        internal static Coroutine WaitForSecondsRealtime(this MonoBehaviour @this, float seconds, Action callback) =>
            @this.StartCoroutine(Process(new WaitForSecondsRealtime(seconds), callback));

        /// <summary> Вызов метода после того, как условие станет false. </summary>
        internal static Coroutine WaitWhile(this MonoBehaviour @this, Func<bool> predicate, Action callback) =>
            @this.StartCoroutine(Process(new WaitWhile(predicate), callback));

        /// <summary> Вызов метода после того, как условие станет true. </summary>
        internal static Coroutine WaitUntil(this MonoBehaviour @this, Func<bool> predicate, Action callback) =>
            @this.StartCoroutine(Process(new WaitUntil(predicate), callback));

        /// <summary> Вызов метода в начале кадра. </summary>
        internal static Coroutine WaitUpdate(this MonoBehaviour @this, Action callback) =>
            @this.StartCoroutine(Process((IEnumerator)null, callback));

        /// <summary> Вызов метода в начале фиксированного кадра. </summary>
        internal static Coroutine WaitFixedUpdate(this MonoBehaviour @this, Action callback) =>
            @this.StartCoroutine(Process(Yielders.FixedUpdate, callback));

        /// <summary> Вызов метода в конце кадра. </summary>
        internal static Coroutine WaitEndOfFrame(this MonoBehaviour @this, Action callback) =>
            @this.StartCoroutine(Process(Yielders.EndOfFrame, callback));

        private static IEnumerator Process(IEnumerator yielder, Action callback)
        {
            yield return yielder;
            callback?.Invoke();
        }

        private static IEnumerator Process(YieldInstruction yielder, Action callback)
        {
            yield return yielder;
            callback?.Invoke();
        }
    }
}