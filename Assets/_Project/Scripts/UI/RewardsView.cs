using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;

namespace WheelOfFortune.UI
{
    public class RewardsView : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private GameObject _itemPrefab;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _container = UiBind.Find<Transform>(this, "ui_container_rewards");
        }
#endif

        public void Render(IReadOnlyDictionary<RewardDefinitionSo, int> rewards)
        {
            Populate(_container, _itemPrefab, rewards);
        }

        internal static void Populate(Transform container, GameObject itemPrefab, IReadOnlyDictionary<RewardDefinitionSo, int> rewards)
        {
            for (var i = container.childCount - 1; i >= 0; i--)
            {
                Destroy(container.GetChild(i).gameObject);
            }

            foreach (var reward in rewards)
            {
                var item = Instantiate(itemPrefab, container);
                var icon = item.transform.Find("ui_image_reward_icon_value").GetComponent<Image>();
                var amountText = item.transform.Find("ui_text_reward_amount_value").GetComponent<TMP_Text>();

                icon.sprite = reward.Key.Icon;
                amountText.text = reward.Value.ToString();
            }
        }
    }
}
