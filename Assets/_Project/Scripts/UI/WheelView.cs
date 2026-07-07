using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;

namespace WheelOfFortune.UI
{
    public class WheelView : MonoBehaviour
    {
        [Serializable]
        private class SliceRefs
        {
            [SerializeField] private Image _icon;
            [SerializeField] private TMP_Text _amountText;

            public Image Icon => _icon;
            public TMP_Text AmountText => _amountText;
        }

        [SerializeField] private Image _baseImage;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Sprite _bombIconSprite;
        [SerializeField] private SliceRefs[] _slices = new SliceRefs[8];

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
