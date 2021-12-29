using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;
using Fungus.PlaytimeSys;

namespace Fungus.Tests
{
    public class PlaytimeTrackerTests
    {
        
        [SetUp]
        public virtual void SetUp()
        {
            scenePrefab = Resources.Load<GameObject>(prefabPath);
            sceneObject = GameObject.Instantiate<GameObject>(scenePrefab, Vector3.zero, Quaternion.identity);
        }

        string prefabPath = "Prefabs/PlaytimeTrackerTests";
        GameObject scenePrefab, sceneObject;

        public virtual IEnumerator SetUpForCoroutineTests()
        {
            yield return new WaitForSeconds(0.5f);
            tracker = FungusManager.Instance.PlaytimeTracker;
        }

        PlaytimeTracker tracker;

        [UnityTest]
        public virtual IEnumerator TracksSecondsProperly()
        {
            yield return SetUpForCoroutineTests();

            int expectedSeconds = 2;
            tracker.StartTracking();
            float timeOffset = 0.2f;
            // ^ WaitForSeconds sometimes waits for ever-so-slightly less than the amount passed to it, which can mess up
            // the results of this test. Hence the offset to fix that issue
            yield return new WaitForSeconds(expectedSeconds + timeOffset);

            TimeSpan timeTracked = tracker.PlaytimeRecorded;
            bool trackedCorrectly = timeTracked.Seconds == expectedSeconds;

            Assert.IsTrue(trackedCorrectly);
        }

        [UnityTest]
        public virtual IEnumerator StopsTrackingOnRequest()
        {
            yield return SetUpForCoroutineTests();

            tracker.StartTracking();
            yield return new WaitForSeconds(1.2f);

            TimeSpan timeTrackedBeforeStop = tracker.PlaytimeRecorded;
            tracker.StopTracking();
            yield return new WaitForSeconds(1.2f);

            TimeSpan timeTrackedAfterStop = tracker.PlaytimeRecorded;
            bool success = timeTrackedAfterStop.Seconds == timeTrackedBeforeStop.Seconds;
            // ^Since the time tracked shouldn't change during do-not-track mode

            Assert.IsTrue(success);
        }
        
    }
}
