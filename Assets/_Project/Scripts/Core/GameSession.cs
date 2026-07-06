using System;
using System.Collections.Generic;
using WheelOfFortune.Data;

namespace WheelOfFortune.Core
{
    public enum SessionState
    {
        Idle,
        Spinning,
        BombExploded,
        Finished
    }

    public class GameSession
    {
        private readonly GameConfigSo _config;
        private readonly Random _random;
        private readonly RewardInventory _inventory = new RewardInventory();
        private SpinResult? _pendingResult;

        public GameSession(GameConfigSo config, Random random)
        {
            _config = config;
            _random = random;
            CurrentZone = 1;
            State = SessionState.Idle;
            CurrentZoneType = ZoneRules.GetZoneType(CurrentZone, _config.SafeZoneInterval, _config.SuperZoneInterval);
            CurrentWheel = GetWheelForZoneType(CurrentZoneType);
        }

        public SessionState State { get; private set; }
        public int CurrentZone { get; private set; }
        public ZoneType CurrentZoneType { get; private set; }
        public WheelConfigSo CurrentWheel { get; private set; }
        public IReadOnlyDictionary<RewardDefinitionSo, int> Rewards => _inventory.Amounts;

        public event Action<SpinResult> SpinResolved;
        public event Action RewardsChanged;
        public event Action ZoneChanged;
        public event Action BombHit;
        public event Action SessionFinished;

        public SpinResult Spin()
        {
            if (State != SessionState.Idle)
            {
                throw new InvalidOperationException($"Cannot spin while state is {State}.");
            }

            var slices = CurrentWheel.Slices;
            var sliceIndex = _random.Next(slices.Length);
            var slice = slices[sliceIndex];

            var amount = 0;
            if (!slice.IsBomb)
            {
                var multiplier = _config.RewardMultiplierByZone.Evaluate(CurrentZone);
                amount = Math.Max(1, (int)Math.Round(slice.BaseAmount * multiplier));
            }

            var result = new SpinResult(sliceIndex, slice, amount);
            _pendingResult = result;
            State = SessionState.Spinning;
            SpinResolved?.Invoke(result);
            return result;
        }

        public void CompleteSpin()
        {
            if (State != SessionState.Spinning || _pendingResult == null)
            {
                throw new InvalidOperationException($"Cannot complete a spin while state is {State}.");
            }

            var result = _pendingResult.Value;
            _pendingResult = null;

            if (result.Slice.IsBomb)
            {
                _inventory.Clear();
                State = SessionState.BombExploded;
                RewardsChanged?.Invoke();
                BombHit?.Invoke();
                return;
            }

            _inventory.Add(result.Slice.Reward, result.ScaledAmount);
            RewardsChanged?.Invoke();

            CurrentZone++;
            CurrentZoneType = ZoneRules.GetZoneType(CurrentZone, _config.SafeZoneInterval, _config.SuperZoneInterval);
            CurrentWheel = GetWheelForZoneType(CurrentZoneType);
            State = SessionState.Idle;
            ZoneChanged?.Invoke();
        }

        public void LeaveWithRewards()
        {
            if (State != SessionState.Idle)
            {
                throw new InvalidOperationException($"Cannot leave while state is {State}.");
            }

            if (CurrentZoneType == ZoneType.Normal)
            {
                throw new InvalidOperationException("Cannot leave on a normal zone.");
            }

            State = SessionState.Finished;
            SessionFinished?.Invoke();
        }

        public void Restart()
        {
            if (State != SessionState.BombExploded && State != SessionState.Finished)
            {
                throw new InvalidOperationException($"Cannot restart while state is {State}.");
            }

            _inventory.Clear();
            CurrentZone = 1;
            CurrentZoneType = ZoneRules.GetZoneType(CurrentZone, _config.SafeZoneInterval, _config.SuperZoneInterval);
            CurrentWheel = GetWheelForZoneType(CurrentZoneType);
            State = SessionState.Idle;
            RewardsChanged?.Invoke();
            ZoneChanged?.Invoke();
        }

        private WheelConfigSo GetWheelForZoneType(ZoneType zoneType)
        {
            switch (zoneType)
            {
                case ZoneType.Super:
                    return _config.GoldenWheel;
                case ZoneType.Safe:
                    return _config.SilverWheel;
                default:
                    return _config.BronzeWheel;
            }
        }
    }
}
