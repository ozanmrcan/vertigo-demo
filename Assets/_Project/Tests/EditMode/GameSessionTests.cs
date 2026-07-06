using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using WheelOfFortune.Core;
using WheelOfFortune.Data;
using Random = System.Random;

namespace WheelOfFortune.Tests
{
    public class GameSessionTests
    {
        private static RewardDefinitionSo CreateReward()
        {
            return ScriptableObject.CreateInstance<RewardDefinitionSo>();
        }

        private static WheelSliceData CreateSlice(RewardDefinitionSo reward, int baseAmount, bool isBomb = false)
        {
            return new WheelSliceData(reward, baseAmount, isBomb);
        }

        private static WheelConfigSo CreateWheel(params WheelSliceData[] slices)
        {
            var wheel = ScriptableObject.CreateInstance<WheelConfigSo>();
            wheel.InitializeForTests(WheelTier.Bronze, slices);
            return wheel;
        }

        private static GameConfigSo CreateConfig(
            int safeInterval,
            int superInterval,
            WheelConfigSo wheel,
            float rewardMultiplier = 1f)
        {
            var config = ScriptableObject.CreateInstance<GameConfigSo>();
            var curve = AnimationCurve.Constant(0f, 1000f, rewardMultiplier);
            config.InitializeForTests(safeInterval, superInterval, wheel, wheel, wheel, curve);
            return config;
        }

        [Test]
        public void Spin_ThrowsWhenNotIdle()
        {
            var wheel = CreateWheel(CreateSlice(CreateReward(), 100));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel);
            var session = new GameSession(config, new Random(1));
            session.Spin();

            Assert.Throws<InvalidOperationException>(() => session.Spin());
        }

        [Test]
        public void CompleteSpin_ThrowsWhenNotSpinning()
        {
            var wheel = CreateWheel(CreateSlice(CreateReward(), 100));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel);
            var session = new GameSession(config, new Random(1));

            Assert.Throws<InvalidOperationException>(() => session.CompleteSpin());
        }

        [Test]
        public void Restart_ThrowsWhenIdle()
        {
            var wheel = CreateWheel(CreateSlice(CreateReward(), 100));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel);
            var session = new GameSession(config, new Random(1));

            Assert.Throws<InvalidOperationException>(() => session.Restart());
        }

        [Test]
        public void CompleteSpin_OnBomb_ClearsInventoryAndSetsBombExploded()
        {
            var wheel = CreateWheel(CreateSlice(null, 0, isBomb: true));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel);
            var session = new GameSession(config, new Random(1));

            session.Spin();
            session.CompleteSpin();

            Assert.AreEqual(SessionState.BombExploded, session.State);
            Assert.AreEqual(0, session.Rewards.Count);
        }

        [Test]
        public void Spin_ThrowsAfterBomb_UntilRestart()
        {
            var wheel = CreateWheel(CreateSlice(null, 0, isBomb: true));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel);
            var session = new GameSession(config, new Random(1));
            session.Spin();
            session.CompleteSpin();

            Assert.Throws<InvalidOperationException>(() => session.Spin());

            session.Restart();

            Assert.AreEqual(SessionState.Idle, session.State);
            Assert.AreEqual(1, session.CurrentZone);
            Assert.DoesNotThrow(() => session.Spin());
        }

        [Test]
        public void LeaveWithRewards_ThrowsOnNormalZone()
        {
            var wheel = CreateWheel(CreateSlice(CreateReward(), 100));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel);
            var session = new GameSession(config, new Random(1));

            Assert.Throws<InvalidOperationException>(() => session.LeaveWithRewards());
        }

        [Test]
        public void LeaveWithRewards_ThrowsWhileSpinning()
        {
            var wheel = CreateWheel(CreateSlice(CreateReward(), 100));
            var config = CreateConfig(safeInterval: 1, superInterval: 100, wheel);
            var session = new GameSession(config, new Random(1));
            session.Spin();

            Assert.Throws<InvalidOperationException>(() => session.LeaveWithRewards());
        }

        [Test]
        public void LeaveWithRewards_SucceedsOnSafeZone()
        {
            var wheel = CreateWheel(CreateSlice(CreateReward(), 100));
            var config = CreateConfig(safeInterval: 1, superInterval: 100, wheel);
            var session = new GameSession(config, new Random(1));

            session.LeaveWithRewards();

            Assert.AreEqual(SessionState.Finished, session.State);
        }

        [Test]
        public void LeaveWithRewards_SucceedsOnSuperZone()
        {
            var wheel = CreateWheel(CreateSlice(CreateReward(), 100));
            var config = CreateConfig(safeInterval: 100, superInterval: 1, wheel);
            var session = new GameSession(config, new Random(1));

            session.LeaveWithRewards();

            Assert.AreEqual(SessionState.Finished, session.State);
        }

        [Test]
        public void CompleteSpin_AccumulatesRewardAcrossMultipleSpins()
        {
            var reward = CreateReward();
            var wheel = CreateWheel(CreateSlice(reward, 100));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel, rewardMultiplier: 1f);
            var session = new GameSession(config, new Random(1));

            session.Spin();
            session.CompleteSpin();
            session.Spin();
            session.CompleteSpin();

            Assert.AreEqual(200, session.Rewards[reward]);
        }

        [Test]
        public void Spin_ScalesRewardByZoneCurve()
        {
            var reward = CreateReward();
            var wheel = CreateWheel(CreateSlice(reward, 100));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel, rewardMultiplier: 1.5f);
            var session = new GameSession(config, new Random(1));

            var result = session.Spin();

            Assert.AreEqual(150, result.ScaledAmount);
        }

        [Test]
        public void Spin_RewardAmount_NeverBelowOne()
        {
            var reward = CreateReward();
            var wheel = CreateWheel(CreateSlice(reward, 1));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel, rewardMultiplier: 0.01f);
            var session = new GameSession(config, new Random(1));

            var result = session.Spin();

            Assert.AreEqual(1, result.ScaledAmount);
        }

        [Test]
        public void Spin_SameSeed_ProducesSameSliceSequence()
        {
            var wheel = CreateWheel(
                CreateSlice(CreateReward(), 10),
                CreateSlice(CreateReward(), 20),
                CreateSlice(CreateReward(), 30),
                CreateSlice(CreateReward(), 40),
                CreateSlice(CreateReward(), 50));
            var configA = CreateConfig(safeInterval: 1000, superInterval: 10000, wheel);
            var configB = CreateConfig(safeInterval: 1000, superInterval: 10000, wheel);

            var sessionA = new GameSession(configA, new Random(42));
            var sessionB = new GameSession(configB, new Random(42));

            var indicesA = new List<int>();
            var indicesB = new List<int>();

            for (var i = 0; i < 5; i++)
            {
                indicesA.Add(sessionA.Spin().SliceIndex);
                sessionA.CompleteSpin();

                indicesB.Add(sessionB.Spin().SliceIndex);
                sessionB.CompleteSpin();
            }

            CollectionAssert.AreEqual(indicesA, indicesB);
        }
    }
}
