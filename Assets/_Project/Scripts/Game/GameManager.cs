using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Core;
using WheelOfFortune.Data;
using WheelOfFortune.UI;

namespace WheelOfFortune.Game
{
    public class GameManager : MonoBehaviour
    {
        private const string GoldPrefsKey = "PersistentGold";

        [SerializeField] private GameConfigSo _config;
        [SerializeField] private WheelView _wheelView;
        [SerializeField] private WheelSpinAnimator _wheelSpinAnimator;
        [SerializeField] private ZonesView _zonesView;
        [SerializeField] private RewardsView _rewardsView;
        [SerializeField] private BombPopupView _bombPopupView;
        [SerializeField] private FinishPopupView _finishPopupView;
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _leaveButton;

        private GameSession _session;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _wheelView = UiBind.Find<WheelView>(this, "ui_panel_wheel");
            _wheelSpinAnimator = UiBind.Find<WheelSpinAnimator>(this, "ui_transform_wheel_rotator");
            _zonesView = UiBind.Find<ZonesView>(this, "ui_panel_zones");
            _rewardsView = UiBind.Find<RewardsView>(this, "ui_panel_rewards");
            _bombPopupView = UiBind.Find<BombPopupView>(this, "ui_panel_popup_bomb");
            _finishPopupView = UiBind.Find<FinishPopupView>(this, "ui_panel_popup_finish");
            _spinButton = UiBind.Find<Button>(this, "ui_button_spin");
            _leaveButton = UiBind.Find<Button>(this, "ui_button_leave");
        }
#endif

        private void Awake()
        {
            var startingGold = PlayerPrefs.GetInt(GoldPrefsKey, 0);
            _session = new GameSession(_config, new System.Random(), startingGold);

            _bombPopupView.Hide();
            _finishPopupView.Hide();
            RefreshWheelAndZone();
            RefreshRewards();
            UpdateButtonInteractable();
        }

        private void OnEnable()
        {
            _spinButton.onClick.AddListener(HandleSpinClicked);
            _leaveButton.onClick.AddListener(HandleLeaveClicked);
            _bombPopupView.GiveUpClicked += HandleRestartRequested;
            _bombPopupView.ReviveClicked += HandleReviveRequested;
            _finishPopupView.RestartClicked += HandleRestartRequested;
            _session.RewardsChanged += HandleRewardsChanged;
            _session.ZoneChanged += HandleZoneChanged;
            _session.BombHit += HandleBombHit;
            _session.SessionFinished += HandleSessionFinished;
            _session.Revived += HandleRevived;
        }

        private void OnDisable()
        {
            _spinButton.onClick.RemoveListener(HandleSpinClicked);
            _leaveButton.onClick.RemoveListener(HandleLeaveClicked);
            _bombPopupView.GiveUpClicked -= HandleRestartRequested;
            _bombPopupView.ReviveClicked -= HandleReviveRequested;
            _finishPopupView.RestartClicked -= HandleRestartRequested;
            _session.RewardsChanged -= HandleRewardsChanged;
            _session.ZoneChanged -= HandleZoneChanged;
            _session.BombHit -= HandleBombHit;
            _session.SessionFinished -= HandleSessionFinished;
            _session.Revived -= HandleRevived;
        }

        private void HandleSpinClicked()
        {
            var result = _session.Spin();
            UpdateButtonInteractable();
            _wheelSpinAnimator.SpinTo(result.SliceIndex, () => HandleSpinLanded(result));
        }

        private void HandleSpinLanded(SpinResult result)
        {
            if (result.Slice.IsBomb)
            {
                _session.CompleteSpin();
                return;
            }

            var icon = _wheelView.GetSliceIcon(result.SliceIndex);
            PlayRewardFly(icon.sprite, icon.rectTransform, _rewardsView.Container, () => _session.CompleteSpin());
        }

        private void PlayRewardFly(Sprite sprite, RectTransform from, RectTransform to, System.Action onComplete)
        {
            var flyObject = new GameObject("ui_image_reward_fly_value", typeof(RectTransform), typeof(Image));
            var flyRect = (RectTransform)flyObject.transform;
            flyRect.SetParent(transform, false);
            flyRect.sizeDelta = new Vector2(80f, 80f);
            flyRect.position = from.position;

            var image = flyObject.GetComponent<Image>();
            image.sprite = sprite;
            image.raycastTarget = false;
            image.preserveAspect = true;

            flyRect.DOScale(0.5f, 0.45f).SetEase(Ease.InQuad);
            flyRect.DOMove(to.position, 0.45f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                Destroy(flyObject);
                onComplete?.Invoke();
            });
        }

        private void HandleLeaveClicked()
        {
            _session.LeaveWithRewards();
        }

        private void HandleRewardsChanged()
        {
            RefreshRewards();
            PersistGold();
        }

        private void HandleZoneChanged()
        {
            RefreshWheelAndZone();
            UpdateButtonInteractable();
        }

        private void HandleBombHit()
        {
            UpdateButtonInteractable();
            var cost = _config.ReviveCost;
            _bombPopupView.Show(cost, _session.PersistentGold >= cost);
        }

        private void HandleReviveRequested()
        {
            _bombPopupView.Hide();
            _session.Revive(_config.ReviveCost);
        }

        private void HandleRevived()
        {
            UpdateButtonInteractable();
            PersistGold();
        }

        private void HandleSessionFinished()
        {
            UpdateButtonInteractable();
            _finishPopupView.Show(_session.Rewards);
        }

        private void HandleRestartRequested()
        {
            _bombPopupView.Hide();
            _finishPopupView.Hide();
            _wheelSpinAnimator.ResetRotation();
            _session.Restart();
        }

        private void RefreshWheelAndZone()
        {
            var multiplier = _config.RewardMultiplierByZone.Evaluate(_session.CurrentZone);
            _wheelView.Bind(_session.CurrentWheel, multiplier);
            _zonesView.Render(_session.CurrentZone, _config.SafeZoneInterval, _config.SuperZoneInterval);
        }

        private void RefreshRewards()
        {
            _rewardsView.Render(_session.Rewards);
        }

        private void UpdateButtonInteractable()
        {
            var idle = _session.State == SessionState.Idle;
            _spinButton.interactable = idle;
            _leaveButton.interactable = idle && _session.CurrentZoneType != ZoneType.Normal;
        }

        private void PersistGold()
        {
            PlayerPrefs.SetInt(GoldPrefsKey, _session.PersistentGold);
            PlayerPrefs.Save();
        }
    }
}
