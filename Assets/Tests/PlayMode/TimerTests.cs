using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus.TimeSys;
using TimeSpan = System.TimeSpan;

namespace Fungus.Tests.TimerSystemTests
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

        [UnityTest]
        public IEnumerator TracksSecondsProperly()
        {
            // Arrange
            int testID = 999;
            timerManager.SetModeOfTimerWithID(testID, TimerMode.countup);
            timerManager.StartTimerWithID(testID);
            
            int expectedSeconds = 2;
            float timeOffset = 0.2f; // <- To account for how imprecise WaitForSeconds can be

            // Act
            yield return new WaitForSeconds(expectedSeconds + timeOffset);

            Timer timerWeUsed = timerManager.Timers[testID];
            TimeSpan timeRecorded = timerWeUsed.TimeRecorded;

            bool success = timeRecorded.Seconds == expectedSeconds;

            // Assert
            Assert.IsTrue(success);
        }
    }
}
