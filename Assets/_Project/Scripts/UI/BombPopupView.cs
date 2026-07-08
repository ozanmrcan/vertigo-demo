using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WheelOfFortune.UI
{
    public class BombPopupView : MonoBehaviour
    {
        private const float AnimDuration = 0.25f;

        [SerializeField] private Button _giveUpButton;
        [SerializeField] private Button _reviveButton;
        [SerializeField] private TMP_Text _reviveCostText;
        [SerializeField] private RectTransform _frame;
        [SerializeField] private CanvasGroup _frameCanvasGroup;

        public event Action GiveUpClicked;
        public event Action ReviveClicked;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _giveUpButton = UiBind.Find<Button>(this, "ui_button_popup_giveup");
            _reviveButton = UiBind.Find<Button>(this, "ui_button_popup_revive");
            _reviveCostText = UiBind.Find<TMP_Text>(this, "ui_text_popup_revive_cost_value");
            _frame = UiBind.Find<RectTransform>(this, "ui_image_popup_frame");
            _frameCanvasGroup = UiBind.Find<CanvasGroup>(this, "ui_image_popup_frame");
        }
#endif

        private void OnEnable()
        {
            _giveUpButton.onClick.AddListener(HandleGiveUpClicked);
            _reviveButton.onClick.AddListener(HandleReviveClicked);
        }

        private void OnDisable()
        {
            _giveUpButton.onClick.RemoveListener(HandleGiveUpClicked);
            _reviveButton.onClick.RemoveListener(HandleReviveClicked);
        }

        public void Show(int reviveCost, bool canAffordRevive)
        {
            gameObject.SetActive(true);
            _reviveCostText.text = reviveCost.ToString();
            _reviveButton.interactable = canAffordRevive;

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

        private void HandleGiveUpClicked()
        {
            GiveUpClicked?.Invoke();
        }

        private void HandleReviveClicked()
        {
            ReviveClicked?.Invoke();
        }
    }
}
