using System;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public class WheelSliceData
    {
        [SerializeField] private RewardDefinitionSo _reward;
        [SerializeField] private int _baseAmount;
        [SerializeField] private bool _isBomb;

        public RewardDefinitionSo Reward => _reward;
        public int BaseAmount => _baseAmount;
        public bool IsBomb => _isBomb;
    }
}
