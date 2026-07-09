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
        private static RewardDefinitionSo CreateReward(bool isCurrency = false)
        {
            var reward = ScriptableObject.CreateInstance<RewardDefinitionSo>();
            reward.InitializeForTests(isCurrency);
            return reward;
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
            float rewardMultiplier = 1f,
            RewardDefinitionSo goldReward = null,
            int reviveCost = 25)
        {
            var config = ScriptableObject.CreateInstance<GameConfigSo>();
            var curve = AnimationCurve.Constant(0f, 1000f, rewardMultiplier);
            config.InitializeForTests(safeInterval, superInterval, wheel, wheel, wheel, curve, goldReward, reviveCost);
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
        public void CompleteSpin_OnBomb_SetsBombExploded_ButKeepsRewardsUntilRestart()
        {
            var reward = CreateReward();
            var wheel = CreateWheel(CreateSlice(reward, 100), CreateSlice(null, 0, isBomb: true));
            var config = CreateConfig(safeInterval: 1000, superInterval: 10000, wheel);
            var session = new GameSession(config, new Random(1));

            session.Spin();
            session.CompleteSpin();
            var rewardsBeforeBomb = new Dictionary<RewardDefinitionSo, int>(session.Rewards);

            while (session.State != SessionState.BombExploded)
            {
                session.Spin();
                session.CompleteSpin();
                if (session.State == SessionState.Idle)
                {
                    rewardsBeforeBomb = new Dictionary<RewardDefinitionSo, int>(session.Rewards);
                }
            }

            Assert.AreEqual(SessionState.BombExploded, session.State);
            Assert.Greater(session.Rewards.Count, 0);
            CollectionAssert.AreEquivalent(rewardsBeforeBomb, session.Rewards);

            session.Restart();

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
        public void Spin_CurrencyReward_ScalesSmoothlyWithRounding()
        {
            var cash = CreateReward(isCurrency: true);
            var wheel = CreateWheel(CreateSlice(cash, 100));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel, rewardMultiplier: 1.76f);
            var session = new GameSession(config, new Random(1));

            Assert.AreEqual(176, session.Spin().ScaledAmount);
        }

        [Test]
        public void Spin_ItemReward_StepsUpOnlyWhenAWholeUnitIsReached()
        {
            var item = CreateReward(isCurrency: false);
            var wheel = CreateWheel(CreateSlice(item, 1));

            var belowWhole = CreateConfig(safeInterval: 5, superInterval: 30, wheel, rewardMultiplier: 1.76f);
            Assert.AreEqual(1, new GameSession(belowWhole, new Random(1)).Spin().ScaledAmount);

            var atWhole = CreateConfig(safeInterval: 5, superInterval: 30, wheel, rewardMultiplier: 2f);
            Assert.AreEqual(2, new GameSession(atWhole, new Random(1)).Spin().ScaledAmount);
        }

        [Test]
        public void Spin_ItemReward_HigherBaseStillGrowsBetweenWholeMultipliers()
        {
            var item = CreateReward(isCurrency: false);
            var wheel = CreateWheel(CreateSlice(item, 5));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel, rewardMultiplier: 1.76f);
            var session = new GameSession(config, new Random(1));

            // floor(5 * 1.76) = floor(8.8) = 8 — a bigger base keeps improving each zone.
            Assert.AreEqual(8, session.Spin().ScaledAmount);
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

        [Test]
        public void CompleteSpin_OnGoldReward_AccumulatesGold_SurvivingABomb()
        {
            var gold = CreateReward();
            var wheel = CreateWheel(CreateSlice(gold, 100), CreateSlice(null, 0, isBomb: true));
            var config = CreateConfig(safeInterval: 1000, superInterval: 10000, wheel, goldReward: gold);
            var session = new GameSession(config, new Random(1));

            session.Spin();
            session.CompleteSpin();

            Assert.AreEqual(100, session.CurrentGold);
            var goldBeforeBomb = session.CurrentGold;

            while (session.State != SessionState.BombExploded)
            {
                session.Spin();
                session.CompleteSpin();
                if (session.State == SessionState.Idle)
                {
                    goldBeforeBomb = session.CurrentGold;
                }
            }

            // Hitting the bomb does not clear the run yet — gold is still available to fund a revive.
            Assert.AreEqual(SessionState.BombExploded, session.State);
            Assert.AreEqual(goldBeforeBomb, session.CurrentGold);
        }

        [Test]
        public void Revive_WhenAffordable_RestoresIdle_KeepsRewardsAndZone_DeductsGold()
        {
            var gold = CreateReward();
            var wheel = CreateWheel(CreateSlice(gold, 100), CreateSlice(null, 0, isBomb: true));
            var config = CreateConfig(safeInterval: 1000, superInterval: 10000, wheel, goldReward: gold, reviveCost: 25);
            var session = new GameSession(config, new Random(1));

            while (session.State != SessionState.BombExploded)
            {
                session.Spin();
                session.CompleteSpin();
            }

            var zoneBeforeRevive = session.CurrentZone;
            var goldBeforeRevive = session.CurrentGold;
            Assert.GreaterOrEqual(goldBeforeRevive, 25);

            session.Revive(25);

            Assert.AreEqual(SessionState.Idle, session.State);
            Assert.AreEqual(zoneBeforeRevive, session.CurrentZone);
            Assert.AreEqual(goldBeforeRevive - 25, session.CurrentGold);
            Assert.Greater(session.Rewards.Count, 0);
        }

        [Test]
        public void Revive_ThrowsWhenGoldInsufficient()
        {
            var wheel = CreateWheel(CreateSlice(null, 0, isBomb: true));
            var config = CreateConfig(safeInterval: 1000, superInterval: 10000, wheel, reviveCost: 25);
            var session = new GameSession(config, new Random(1));

            session.Spin();
            session.CompleteSpin();

            Assert.Throws<InvalidOperationException>(() => session.Revive(25));
            Assert.AreEqual(SessionState.BombExploded, session.State);
        }

        [Test]
        public void Revive_ThrowsWhenNotBombExploded()
        {
            var wheel = CreateWheel(CreateSlice(CreateReward(), 100));
            var config = CreateConfig(safeInterval: 5, superInterval: 30, wheel);
            var session = new GameSession(config, new Random(1));

            Assert.Throws<InvalidOperationException>(() => session.Revive(25));
        }
    }
}
