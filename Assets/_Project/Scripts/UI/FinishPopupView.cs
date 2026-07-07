using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;

namespace WheelOfFortune.UI
{
    public class FinishPopupView : MonoBehaviour
    {
        [SerializeField] private Transform _rewardsContainer;
        [SerializeField] private GameObject _rewardItemPrefab;
        [SerializeField] private Button _restartButton;

        public event Action RestartClicked;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _rewardsContainer = UiBind.Find<Transform>(this, "ui_container_finish_rewards");
            _restartButton = UiBind.Find<Button>(this, "ui_button_popup_restart");
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
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void HandleRestartClicked()
        {
            RestartClicked?.Invoke();
        }
    }
}
