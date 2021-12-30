using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus.TimeSys;
using TimeSpan = System.TimeSpan;

namespace Fungus.Tests.TimeSystemTests
{
    public class TimerTests
    {
        [SetUp]
        protected virtual void SetUp()
        {
            managerPrefab = Resources.Load<GameObject>(managerPrefabPath);
            managerGO = MonoBehaviour.Instantiate<GameObject>(managerPrefab, Vector3.zero, Quaternion.identity);
            timerManager = managerGO.GetComponent<TimerManager>();
        }

        protected string managerPrefabPath = "Prefabs/TimerManager";
        protected GameObject managerPrefab, managerGO;
        TimerManager timerManager;

        [TearDown]
        protected virtual void TearDown()
        {
            MonoBehaviour.Destroy(managerGO.gameObject);
        }

        protected virtual IEnumerator SetUpForCountupTimerTesting()
        {
            // To make sure that the test timer exists and has a clean slate
            timerManager.SetModeOfTimerWithID(testID, TimerMode.countup);
            RegisterAndSetTestTimerToHaveCleanSlate();
            yield return null;
        }

        protected virtual void RegisterAndSetTestTimerToHaveCleanSlate()
        {
            timerManager.StopTimerWithID(testID);
            timerManager.ResetTimerWithID(testID);
            testTimer = timerManager.Timers[testID];
        }

        protected int testID = 999;
        protected Timer testTimer;

        [UnityTest]
        public virtual IEnumerator CountsUpCorrectly()
        {
            // Arrange
            yield return SetUpForCountupTimerTesting();
            int expectedSeconds = 2;

            // Act
            yield return WaitAndLetTimerRunFor(expectedSeconds);

            TimeSpan timeRecorded = testTimer.TimeRecorded;
            bool success = timeRecorded.Seconds == expectedSeconds;

            // Assert
            Assert.IsTrue(success);
        }

        protected virtual IEnumerator WaitAndLetTimerRunFor(int expectedSeconds)
        {
            timerManager.StartTimerWithID(testID);
            yield return new WaitForSeconds(expectedSeconds + timeOffset);
        }

        float timeOffset = 0.2f; // <- To account for how imprecise WaitForSeconds can be

        protected virtual IEnumerator WaitAndLetTimerRunFor(float expectedSeconds)
        {
            timerManager.StartTimerWithID(testID);
            yield return new WaitForSeconds(expectedSeconds + timeOffset);
        }

        [UnityTest]
        public virtual IEnumerator StopsCountupTrackingOnRequest()
        {
            // Arrange
            yield return SetUpForCountupTimerTesting();

            int expectedSeconds = 2;

            // Act
            yield return WaitAndLetTimerRunFor(expectedSeconds);
            timerManager.StopTimerWithID(testID);

            yield return WaitAndLetTimerRunFor(expectedSeconds);
            // ^This should be ignored by the timer

            TimeSpan timeRecorded = testTimer.TimeRecorded;
            bool success = timeRecorded.Seconds == expectedSeconds;

            Assert.IsTrue(success);

        }

        [UnityTest]
        public virtual IEnumerator ResetsCountupTrackingOnRequest()
        {
            // Arrange
            yield return SetUpForCountupTimerTesting();

            int secondsToWait = 1, expectedSeconds = 0;

            // Act
            yield return WaitAndLetTimerRunFor(secondsToWait);
            timerManager.ResetTimerWithID(testID);

            TimeSpan timeRecorded = testTimer.TimeRecorded;

            bool success = timeRecorded.Seconds == expectedSeconds;

            Assert.IsTrue(success);

        }

        [UnityTest]
        public virtual IEnumerator CountsDownCorrectly()
        {
            // Arrange
            yield return SetUpForCountdownTimerTesting();

            int timeToWait = 2;
            int expectedRemainingSeconds = countdownStartTime.Seconds - timeToWait;

            // Act
            yield return WaitAndLetTimerRunFor(timeToWait - timeOffset);
            // ^ We are negating the time offset since for some reason, the timer is more precise when counting down
            TimeSpan remainingTime = testTimer.TimeRecorded;

            // Assert
            bool success = remainingTime.Seconds == expectedRemainingSeconds;
            Assert.IsTrue(success);

        }

        protected virtual IEnumerator SetUpForCountdownTimerTesting()
        {
            // To make sure that the test timer exists and has a clean slate
            timerManager.SetModeOfTimerWithID(testID, TimerMode.countdown);
            timerManager.SetCountdownStartingTimeOfTimerWithID(testID, ref countdownStartTime);
            RegisterAndSetTestTimerToHaveCleanSlate();
            yield return null;
        }

        protected TimeSpan countdownStartTime = new TimeSpan(0, 0, 5);
    }
}
