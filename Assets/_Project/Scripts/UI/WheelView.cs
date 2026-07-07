using System;
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
        [SerializeField] private Sprite _bombIconSprite;
        [SerializeField] private SliceRefs[] _slices = new SliceRefs[SliceCount];

#if UNITY_EDITOR
        private void OnValidate()
        {
            _baseImage = UiBind.Find<Image>(this, "ui_image_wheel_base_value");
            _indicatorImage = UiBind.Find<Image>(this, "ui_image_wheel_indicator_value");
            _titleText = UiBind.Find<TMP_Text>(this, "ui_text_wheel_title_value");

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

        public void Bind(WheelConfigSo config)
        {
            _baseImage.sprite = config.BaseSprite;
            _indicatorImage.sprite = config.IndicatorSprite;
            _titleText.text = config.Title;

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
    }
}
