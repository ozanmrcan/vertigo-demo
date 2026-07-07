using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WheelOfFortune.Core;

namespace WheelOfFortune.UI
{
    public class ZonesView : MonoBehaviour
    {
        private const int WindowSize = 5;
        private const int HalfWindow = WindowSize / 2;

        [SerializeField] private Transform _container;
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _safeSprite;
        [SerializeField] private Sprite _superSprite;
        [SerializeField] private Sprite _currentSprite;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _container = UiBind.Find<Transform>(this, "ui_container_zones");
        }
#endif

        public void Render(int currentZone, int safeInterval, int superInterval)
        {
            for (var i = _container.childCount - 1; i >= 0; i--)
            {
                Destroy(_container.GetChild(i).gameObject);
            }

            var start = Mathf.Max(1, currentZone - HalfWindow);
            for (var zone = start; zone < start + WindowSize; zone++)
            {
                var item = Instantiate(_itemPrefab, _container);

                // ui_text_zone_value is nested inside ui_image_zone_bg_value, not a sibling of it.
                var bgTransform = item.transform.Find("ui_image_zone_bg_value");
                var bg = bgTransform.GetComponent<Image>();
                var text = bgTransform.Find("ui_text_zone_value").GetComponent<TMP_Text>();

                bg.sprite = zone == currentZone ? _currentSprite : SpriteForZoneType(zone, safeInterval, superInterval);
                text.text = zone.ToString();
            }
        }

        private Sprite SpriteForZoneType(int zone, int safeInterval, int superInterval)
        {
            switch (ZoneRules.GetZoneType(zone, safeInterval, superInterval))
            {
                case ZoneType.Super:
                    return _superSprite;
                case ZoneType.Safe:
                    return _safeSprite;
                default:
                    return _normalSprite;
            }
        }
    }
}
