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
        [SerializeField] private Sprite _currentSprite;

        // No dedicated gold zone sprite ships with the assets, so super zones reuse the neutral
        // panel (the sprite safe zones use) tinted gold via Image.color. Tunable in the Inspector.
        [SerializeField] private Color _superColor = new Color(1f, 0.84f, 0.4f, 1f);

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

                ApplyZoneStyle(bg, zone, currentZone, safeInterval, superInterval);
                text.text = zone.ToString();
            }
        }

        private void ApplyZoneStyle(Image bg, int zone, int currentZone, int safeInterval, int superInterval)
        {
            var type = ZoneRules.GetZoneType(zone, safeInterval, superInterval);

            // Super wins over the current-zone highlight: a super zone always reads gold, even when
            // it's the zone you're on (otherwise zone 30 would show the cyan "current" sprite).
            if (type == ZoneType.Super)
            {
                bg.sprite = _safeSprite;
                bg.color = _superColor;
                return;
            }

            bg.color = Color.white;
            bg.sprite = zone == currentZone
                ? _currentSprite
                : (type == ZoneType.Safe ? _safeSprite : _normalSprite);
        }
    }
}
