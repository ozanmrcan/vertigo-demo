using UnityEngine;
using WheelOfFortune.Data;

namespace WheelOfFortune.UI
{
    // Temporary Phase 7 harness for verifying spin-to-index math and wheel binding visually
    // before GameSession drives both for real in Phase 9. Delete this file (and its scene
    // object) in Phase 9.
    public class WheelSpinDebugDriver : MonoBehaviour
    {
        [SerializeField] private WheelView _view;
        [SerializeField] private WheelSpinAnimator _animator;
        [SerializeField] private int _sliceIndex;
        [SerializeField] private WheelConfigSo _bronzeWheel;
        [SerializeField] private WheelConfigSo _silverWheel;
        [SerializeField] private WheelConfigSo _goldenWheel;

        [ContextMenu("Spin To Slice Index")]
        private void SpinToSliceIndex()
        {
            var index = _sliceIndex;
            _animator.SpinTo(index, () => Debug.Log($"Landed on slice {index}."));
        }

        [ContextMenu("Bind Bronze Wheel")]
        private void BindBronzeWheel() => _view.Bind(_bronzeWheel);

        [ContextMenu("Bind Silver Wheel")]
        private void BindSilverWheel() => _view.Bind(_silverWheel);

        [ContextMenu("Bind Golden Wheel")]
        private void BindGoldenWheel() => _view.Bind(_goldenWheel);
    }
}
