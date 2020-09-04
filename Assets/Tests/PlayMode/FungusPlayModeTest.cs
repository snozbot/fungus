// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_2019_2_OR_NEWER

using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace Fungus.Tests
{
    [TestFixture]
    public class FungusPlayModeTest
    {
        [UnityTest]
        public IEnumerator Looping()
        {
            yield return EditorUtils.TestUtils.RunPrefabFlowchartTests("LoopTest", true, 200);
        }

        [UnityTest]
        public IEnumerator ControlFlow()
        {
            yield return EditorUtils.TestUtils.RunPrefabFlowchartTests("FlowTest", true);
        }

        [UnityTest]
        public IEnumerator VariableSets()
        {
            yield return EditorUtils.TestUtils.RunPrefabFlowchartTests("VarSetTest", true);
        }

        [UnityTest]
        public IEnumerator SaveLoadTest()
        {
            yield return EditorUtils.TestUtils.RunPrefabFlowchartTests("SaveTestRoot", false);
        }

        [UnityTest]
        public IEnumerator ManualSaveLoadGlobalVars()
        {
            var gv = FungusManager.Instance.GlobalVariables;
            yield return null;
            gv = FungusManager.Instance.GlobalVariables;

            var saveHandler = DefaultSaveHandler.CreateDefaultWithSerializers();

            var vi1 = gv.GetOrAddVariable<int>("1", 1, typeof(IntegerVariable));
            var vi2 = gv.GetOrAddVariable<int>("2", 2, typeof(IntegerVariable));

            var saveData = saveHandler.CreateSaveData("ManualSaveLoadGlobalVars", "");

            var vi3 = gv.GetOrAddVariable<int>("3", 3, typeof(IntegerVariable));
            vi2.Value = 4;

            saveHandler.LoadSaveData(saveData);

            Assert.AreEqual(gv.GetVariable("1").GetValue(), 1);
            Assert.AreEqual(gv.GetVariable("2").GetValue(), 2);
            Assert.AreEqual(gv.GetVariable("3"), null);
        }

        [UnityTest]
        public IEnumerator ManualSaveLoadFungusSystem()
        {
            var nl = FungusManager.Instance.NarrativeLog;
            yield return null;
            nl = FungusManager.Instance.NarrativeLog;

            nl.AddLine(new NarrativeLogEntry() { name = "Test1", text = "Hello" });
            nl.AddLine(new NarrativeLogEntry() { name = "Test2", text = "World" });

            var nlstate = nl.GetPrettyHistory();

            string startingText = @"This is test [a|b|c]";
            string startingTextA = @"This is test a";
            string startingTextB = @"This is test b";
            string startingTextC = @"This is test c";

            string res = string.Empty;

            res = Fungus.TextVariationHandler.SelectVariations(startingText);
            Assert.AreEqual(res, startingTextA);

            FungusPrioritySignals.DoIncreasePriorityDepth();
            FungusPrioritySignals.DoIncreasePriorityDepth();

            var saveHandler = DefaultSaveHandler.CreateDefaultWithSerializers();
            //post save
            var saveData = saveHandler.CreateSaveData("ManualSaveLoadFungusSystem", "");

            nl.AddLine(new NarrativeLogEntry() { name = "Test3", text = "ERROR!" });
            res = Fungus.TextVariationHandler.SelectVariations(startingText);
            Assert.AreEqual(res, startingTextB);
            res = Fungus.TextVariationHandler.SelectVariations(startingText);
            Assert.AreEqual(res, startingTextC);
            Assert.AreEqual(FungusPrioritySignals.CurrentPriorityDepth, 2);
            FungusPrioritySignals.DoIncreasePriorityDepth();
            Assert.AreEqual(FungusPrioritySignals.CurrentPriorityDepth, 3);
            FungusPrioritySignals.DoResetPriority();
            Assert.AreEqual(FungusPrioritySignals.CurrentPriorityDepth, 0);

            saveHandler.LoadSaveData(saveData);

            //post load
            Assert.AreEqual(nl.GetPrettyHistory(), nlstate);
            res = Fungus.TextVariationHandler.SelectVariations(startingText);
            Assert.AreEqual(res, startingTextB);
            res = Fungus.TextVariationHandler.SelectVariations(startingText);
            Assert.AreEqual(res, startingTextC);
            res = Fungus.TextVariationHandler.SelectVariations(startingText);
            Assert.AreEqual(res, startingTextC);

            Assert.AreEqual(FungusPrioritySignals.CurrentPriorityDepth, 2);
        }
    }
}

#endif