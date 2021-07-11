using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus;

namespace SaveSystemTests
{
    public class SaveDataSoundEncodingTests
    {
        protected string pathToScene = "Prefabs/SoundEncodingTests";
        protected GameObject scenePrefab;
        protected GameObject sceneObj;

        [SetUp]
        public void SetUp()
        {
            // We will assume that some sound will be playing thanks to the Flowcharts in the scene
            scenePrefab = Resources.Load<GameObject>(pathToScene);
            sceneObj = Object.Instantiate<GameObject>(scenePrefab);
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(sceneObj.gameObject);
        }

        [UnityTest]
        public IEnumerator RightAmountOfEncodedData()
        {
            yield return new WaitForSeconds(forSoundToPlay);
            EncodeSound();

            Assert.AreEqual(encodedSoundData.Length, expectedEncodedDataCount);
        }

       

        protected float forSoundToPlay = 0.15f;

        protected void EncodeSound()
        {
            var musicSerializer = new FungusMusicSaveDataItemSerializer();

            encodedSoundData = musicSerializer.Encode();
        }

        protected StringPair[] encodedSoundData;
        protected const int expectedEncodedDataCount = 1;
        // ^While the test scene plays both a BGM and an ambient sound on startup,
        // it puts the data for those things into a single StringPair

        [UnityTest]
        [Ignore("")]
        public IEnumerator EncodedPitchIsCorrect()
        {
            yield return new WaitForSeconds(forSoundToPlay);
            EncodeSound();

        }

        protected const float expectedPitch = 0.238f;
    }
}
