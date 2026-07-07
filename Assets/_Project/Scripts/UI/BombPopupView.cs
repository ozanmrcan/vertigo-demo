using System;
using UnityEngine;
using UnityEngine.UI;

namespace WheelOfFortune.UI
{
    public class BombPopupView : MonoBehaviour
    {
        [SerializeField] private Button _giveUpButton;

        public event Action GiveUpClicked;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _giveUpButton = UiBind.Find<Button>(this, "ui_button_popup_giveup");
        }
#endif

        private void OnEnable()
        {
            _giveUpButton.onClick.AddListener(HandleGiveUpClicked);
        }

        private void OnDisable()
        {
            _giveUpButton.onClick.RemoveListener(HandleGiveUpClicked);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void HandleGiveUpClicked()
        {
            GiveUpClicked?.Invoke();
        }
    }
}
