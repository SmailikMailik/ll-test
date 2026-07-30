using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
#endif

namespace LL.UI.Windows.Reward
{
    internal abstract class RewardAnimation : MonoBehaviour
    {
        private Sequence _sequence;

        private void Awake()
        {
            ApplyHiddenState();
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        internal void Play()
        {
            StopAnimation();
            ApplyHiddenState();
            PlayNestedAnimations();

            _sequence = DOTween.Sequence().SetUpdate(true);
            BuildSequence(_sequence);
            _sequence.Play();
        }

        internal void ResetView()
        {
            StopAnimation();
            ResetNestedAnimations();
            ApplyHiddenState();
        }

        protected abstract void BuildSequence(Sequence sequence);
        protected abstract void ApplyHiddenState();

        protected virtual void PlayNestedAnimations() { }
        protected virtual void ResetNestedAnimations() { }

        private void StopAnimation()
        {
            _sequence?.Kill();
            _sequence = null;
        }

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void DrawTestButtons()
        {
            var isPlaying = Application.isPlaying;

            if (isPlaying is false)
            {
                EditorGUILayout.HelpBox(
                    "Animation testing is available only in Play Mode.",
                    MessageType.Info);
            }

            using (new EditorGUI.DisabledScope(isPlaying is false))
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Play"))
                    Play();

                if (GUILayout.Button("Reset"))
                    ResetView();

                GUILayout.EndHorizontal();
            }
        }
#endif
    }
}