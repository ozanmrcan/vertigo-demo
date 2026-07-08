using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;

namespace WheelOfFortune.UI
{
    public class FinishPopupView : MonoBehaviour
    {
        private const float AnimDuration = 0.25f;

        [SerializeField] private Transform _rewardsContainer;
        [SerializeField] private GameObject _rewardItemPrefab;
        [SerializeField] private Button _restartButton;
        [SerializeField] private RectTransform _frame;
        [SerializeField] private CanvasGroup _frameCanvasGroup;

        public event Action RestartClicked;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _rewardsContainer = UiBind.Find<Transform>(this, "ui_container_finish_rewards");
            _restartButton = UiBind.Find<Button>(this, "ui_button_popup_restart");
            _frame = UiBind.Find<RectTransform>(this, "ui_image_popup_frame");
            _frameCanvasGroup = UiBind.Find<CanvasGroup>(this, "ui_image_popup_frame");
        }
#endif

        private void OnEnable()
        {
            _restartButton.onClick.AddListener(HandleRestartClicked);
        }

        private void OnDisable()
        {
            _restartButton.onClick.RemoveListener(HandleRestartClicked);
        }

        public void Show(IReadOnlyDictionary<RewardDefinitionSo, int> rewards)
        {
            RewardsView.Populate(_rewardsContainer, _rewardItemPrefab, rewards);
            gameObject.SetActive(true);

            DOTween.Kill(_frame);
            DOTween.Kill(_frameCanvasGroup);
            _frame.localScale = Vector3.one * 0.85f;
            _frameCanvasGroup.alpha = 0f;
            _frame.DOScale(1f, AnimDuration).SetEase(Ease.OutBack);
            DOTween.To(() => _frameCanvasGroup.alpha, a => _frameCanvasGroup.alpha = a, 1f, AnimDuration)
                .SetTarget(_frameCanvasGroup);
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            DOTween.Kill(_frame);
            DOTween.Kill(_frameCanvasGroup);
            _frame.DOScale(0.85f, AnimDuration).SetEase(Ease.InQuad);
            DOTween.To(() => _frameCanvasGroup.alpha, a => _frameCanvasGroup.alpha = a, 0f, AnimDuration)
                .SetTarget(_frameCanvasGroup)
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void HandleRestartClicked()
        {
            RestartClicked?.Invoke();
        }
    }
}
