using System;
using DG.Tweening;
using UnityEngine;

namespace WheelOfFortune.UI
{
    public class WheelSpinAnimator : MonoBehaviour
    {
        private const float DegreesPerSlice = 360f / 8;

        [SerializeField] private RectTransform _rotator;
        [SerializeField] private float _spinDuration = 3.5f;
        [SerializeField] private int _extraTurns = 4;

        private float _accumulatedAngle;

        public bool IsSpinning { get; private set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _rotator = GetComponent<RectTransform>();
        }
#endif

        public void SpinTo(int sliceIndex, Action onComplete)
        {
            // Landing angle is sliceIndex * DegreesPerSlice (mod 360): slice 0 sits under the
            // indicator at rest, and index i sits at local angle -i * DegreesPerSlice, so
            // rotating the wheel to +i * DegreesPerSlice brings slice i under the indicator.
            var targetMod = Mod360(sliceIndex * DegreesPerSlice);
            var currentMod = Mod360(_accumulatedAngle);
            var delta = targetMod - currentMod;
            if (delta < 0f)
            {
                delta += 360f;
            }

            _accumulatedAngle += delta + _extraTurns * 360f;

            IsSpinning = true;
            _rotator.DORotate(new Vector3(0f, 0f, _accumulatedAngle), _spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuart)
                .OnComplete(() =>
                {
                    IsSpinning = false;
                    onComplete?.Invoke();
                });
        }

        private static float Mod360(float angle)
        {
            var result = angle % 360f;
            return result < 0f ? result + 360f : result;
        }
    }
}
