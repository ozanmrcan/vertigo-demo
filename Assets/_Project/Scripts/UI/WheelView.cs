using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;

namespace WheelOfFortune.UI
{
    public class WheelView : MonoBehaviour
    {
        private const int SliceCount = 8;

        [Serializable]
        private class SliceRefs
        {
            [SerializeField] private Image _icon;
            [SerializeField] private TMP_Text _amountText;

            public Image Icon => _icon;
            public TMP_Text AmountText => _amountText;

#if UNITY_EDITOR
            public void SetRefs(Image icon, TMP_Text amountText)
            {
                _icon = icon;
                _amountText = amountText;
            }
#endif
        }

        [SerializeField] private Image _baseImage;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _multiplierText;
        [SerializeField] private Image _shineImage;
        [SerializeField] private Sprite _bombIconSprite;
        [SerializeField] private SliceRefs[] _slices = new SliceRefs[SliceCount];

        private bool _shineSpinning;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _baseImage = UiBind.Find<Image>(this, "ui_image_wheel_base_value");
            _indicatorImage = UiBind.Find<Image>(this, "ui_image_wheel_indicator_value");
            _titleText = UiBind.Find<TMP_Text>(this, "ui_text_wheel_title_value");
            _multiplierText = UiBind.Find<TMP_Text>(this, "ui_text_wheel_multiplier_value");
            _shineImage = UiBind.Find<Image>(this, "ui_image_wheel_shine_value");

            if (_slices == null || _slices.Length != SliceCount)
            {
                _slices = new SliceRefs[SliceCount];
            }

            for (var i = 0; i < SliceCount; i++)
            {
                // Unity suffixes sibling duplicates as "name (n)", so slice 0 is unsuffixed and
                // this is also what makes each slice's own container name globally unique -
                // required because the icon/text names *inside* each slice repeat across all 8.
                var sliceName = i == 0 ? "ui_item_slice" : $"ui_item_slice ({i})";
                var sliceRoot = UiBind.Find<Transform>(this, sliceName);
                if (sliceRoot == null)
                {
                    continue;
                }

                _slices[i] ??= new SliceRefs();
                _slices[i].SetRefs(
                    UiBind.Find<Image>(sliceRoot, "ui_image_slice_icon_value"),
                    UiBind.Find<TMP_Text>(sliceRoot, "ui_text_slice_amount_value"));
            }
        }
#endif

        public void Bind(WheelConfigSo config, float rewardMultiplier)
        {
            _baseImage.sprite = config.BaseSprite;
            _indicatorImage.sprite = config.IndicatorSprite;
            _titleText.text = config.Title;
            _titleText.color = TitleColorForTier(config.Tier);
            _multiplierText.text = $"x{rewardMultiplier:0.00}";
            _multiplierText.color = MultiplierColorForTier(config.Tier);

            var showShine = config.Tier == WheelTier.Silver || config.Tier == WheelTier.Golden;
            _shineImage.gameObject.SetActive(showShine);
            if (showShine)
            {
                _shineImage.color = ShineColorForTier(config.Tier);
                if (!_shineSpinning)
                {
                    _shineSpinning = true;
                    _shineImage.rectTransform
                        .DORotate(new Vector3(0f, 0f, 360f), 10f, RotateMode.FastBeyond360)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Restart);
                }
            }

            var sliceData = config.Slices;
            for (var i = 0; i < _slices.Length; i++)
            {
                var slice = sliceData[i];
                var view = _slices[i];

                if (slice.IsBomb)
                {
                    view.Icon.sprite = _bombIconSprite;
                    view.AmountText.text = string.Empty;
                }
                else
                {
                    view.Icon.sprite = slice.Reward.Icon;
                    view.AmountText.text = slice.BaseAmount.ToString();
                }
            }
        }

        public Image GetSliceIcon(int sliceIndex)
        {
            return _slices[sliceIndex].Icon;
        }

        private static Color TitleColorForTier(WheelTier tier)
        {
            switch (tier)
            {
                case WheelTier.Silver:
                    return new Color(0.88f, 0.92f, 1f);
                case WheelTier.Golden:
                    return new Color(1f, 0.9f, 0.6f);
                default:
                    return new Color(1f, 0.75f, 0.45f);
            }
        }

        private static Color ShineColorForTier(WheelTier tier)
        {
            // Silver keeps the sprite's natural white/silver look; golden tints it warm gold.
            // Full alpha so the halo actually reads against the dark background instead of washing out.
            return tier == WheelTier.Golden
                ? new Color(1f, 0.85f, 0.4f, 1f)
                : Color.white;
        }

        private static Color MultiplierColorForTier(WheelTier tier)
        {
            switch (tier)
            {
                case WheelTier.Silver:
                    return new Color(0.75f, 0.85f, 0.95f);
                case WheelTier.Golden:
                    return new Color(1f, 0.75f, 0.25f);
                default:
                    return new Color(1f, 0.55f, 0.25f);
            }
        }
    }
}
