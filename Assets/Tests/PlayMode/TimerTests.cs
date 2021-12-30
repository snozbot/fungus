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
            timerManager.StopTimerWithID(testID);
            timerManager.ResetTimerWithID(testID);
            testTimer = timerManager.Timers[testID];
            yield return null;
        }

        protected int testID = 999;
        protected Timer testTimer;

        [UnityTest]
        public virtual IEnumerator TracksSecondsProperly()
        {
            // Arrange
            yield return SetUpForCountupTimerTesting();
            timerManager.StartTimerWithID(testID);
            
            int expectedSeconds = 2;
            float timeOffset = 0.2f; // <- To account for how imprecise WaitForSeconds can be

            // Act
            yield return new WaitForSeconds(expectedSeconds + timeOffset);

            TimeSpan timeRecorded = testTimer.TimeRecorded;
            bool success = timeRecorded.Seconds == expectedSeconds;

            // Assert
            Assert.IsTrue(success);
        }


        [UnityTest]
        public virtual IEnumerator StopsCountupTrackingOnRequest()
        {
            // Arrange
            yield return SetUpForCountupTimerTesting();
            timerManager.StartTimerWithID(testID);

            int expectedSeconds = 2;
            float timeOffset = 0.2f; // <- To account for how imprecise WaitForSeconds can be

            // Act
            yield return new WaitForSeconds(expectedSeconds + timeOffset);
            timerManager.StopTimerWithID(testID);

            yield return new WaitForSeconds(expectedSeconds + timeOffset);
            // ^This should be ignored by the timer

            TimeSpan timeRecorded = testTimer.TimeRecorded;
            bool success = timeRecorded.Seconds == expectedSeconds;

            Assert.IsTrue(success);

        }
    }
}
