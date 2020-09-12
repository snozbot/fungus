// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Fungus.Tests
{
    [TestFixture]
    public class FungusSaveLoadTests
    {
        [SetUp]
        public void ForceFungusManagerAlive()
        {
            FungusManager.ForceApplicationQuitting(false);
        }

        //by no means exaustive but provides us a basic test in editor mode that flowchart
        // serialisation is at all functional
        [UnityTest]
        public IEnumerator ManualSaveLoadFlowchart()
        {
            const int InitialVariableValue = 8;
            const int ChangedVariableValue = 8;

            //create flowchart
            var go = new GameObject();
            var f = go.AddComponent<Flowchart>();
            var b = f.CreateBlock(new Vector2());
            var vi = f.gameObject.AddComponent<IntegerVariable>();
            vi.Key = f.GetUniqueVariableKey("");
            f.Variables.Add(vi);

            yield return b.Execute(0, null);
            vi.Value = InitialVariableValue;

            var saveHandler = new DefaultSaveHandler();
            var mf = new MultiFlowchartSaveDataItemSerializer();
            saveHandler.SaveDataItemSerializers.Add(mf);
            mf.flowchartsToSave.Add(f);
            var saveData = saveHandler.CreateSaveData("ManualSaveLoadFlowchart", "");

            //change data
            yield return b.Execute(0, null);
            yield return b.Execute(0, null);
            vi.Value = ChangedVariableValue;

            //decode to revert
            saveHandler.LoadSaveData(saveData);

            Assert.That(b.GetExecutionCount() == 1);
            Assert.That(vi.Value == InitialVariableValue);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void ManualSaveLoadValueCollection()
        {
            var go = new GameObject();
            var intcol = go.AddComponent<IntCollection>();

            intcol.Add(1);
            intcol.Add(2);

            var saveHandler = new DefaultSaveHandler();
            var vts = new ValueTypeCollectionSaveDataItemSerializer();
            saveHandler.SaveDataItemSerializers.Add(vts);

            vts.collectionsToSerialize.Add(intcol);
            var saveData = saveHandler.CreateSaveData("ManualSaveLoadValueCollection", "");

            intcol.Remove(1);
            intcol.Add(3);
            intcol.Add(4);

            saveHandler.LoadSaveData(saveData);

            Assert.That(intcol.Count == 2);
            Assert.That(intcol.IndexOf(1) == 0);
            Assert.That(intcol.IndexOf(2) == 1);
        }

        [Test]
        public void UserProfileCycle()
        {
            var upm = new UserProfileManager();
            upm.Init();
            upm.ChangeProfile("test1");
            Assert.AreEqual(upm.CurrentUserProfileName, "test1");
            upm.ChangeProfile("test2");
            Assert.AreEqual(upm.CurrentProfileData.saveName, "test2");
            upm.CurrentProfileData.stringPairs.AddUnique("key", "value");

            upm.SaveProfileData();

            upm = new UserProfileManager();
            Assert.AreEqual(upm.CurrentProfileData, null);
            upm.Init();
            Assert.AreEqual(upm.CurrentProfileData.saveName, "test2");
            Assert.AreEqual(upm.CurrentProfileData.stringPairs.GetOrDefault("key"), "value");
        }

        [UnityTest]
        public IEnumerator SaveManagerManualLoad()
        {
            var upm = new UserProfileManager();
            upm.Init();
            upm.ChangeProfile("save_manager_test");
            yield return null;

            var sm = new SaveFileManager();
            sm.Init(upm);
            sm.DeleteAllSaves();
            Assert.AreEqual(sm.NumSaveMetas, 0);

            var go = new GameObject();
            var intcol = go.AddComponent<IntCollection>();


            var saveHandler = sm.CurrentSaveHandler;
            var vts = new ValueTypeCollectionSaveDataItemSerializer();
            saveHandler.SaveDataItemSerializers.Add(vts);

            vts.collectionsToSerialize.Add(intcol);


            const int MaxSaves = 3;

            for (int i = 0; i < MaxSaves; i++)
            {
                intcol.Add(i);
                sm.Save(i.ToString(), string.Empty, i.ToString() + "_");
            }

            Assert.AreEqual(intcol.Count, 3);
            Assert.AreEqual(sm.NumSaveMetas, MaxSaves);


            //can't use regular load as it uses scenes
            var sd = sm.GetSaveDataFromMeta(sm.SaveNameToMeta("1"));

            sm.CurrentSaveHandler.LoadSaveData(sd);

            Assert.AreEqual(intcol.Count, 2);
        }
    }
}
