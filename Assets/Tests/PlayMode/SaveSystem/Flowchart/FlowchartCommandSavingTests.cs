using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Fungus.LionManeSaveSys;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartCommandSavingTests : SaveSysPlayModeTest
    {
        protected override string PathToScene => "Prefabs/FlowchartSavingTests";

        public override void SetUp()
        {
            base.SetUp();
        }

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator MenuHideIfVisitedSaved()
        {
            // Might want to erase this test later on... The Menu Command tells whether its target block was
            // visited based on said target's execution count rather than some other flag in said Command
            throw new System.NotImplementedException();
        }

        [UnityTest]
        public virtual IEnumerator SayExecutionCountSaved()
        {
            yield return SetUpForEachTest();

            string flowchartName = "ExecutingFlowchart_Dialogue";
            Flowchart flowchartToExecute = GameObject.Find(flowchartName).GetComponent<Flowchart>();
            string blockName = "Dialogue";
            flowchartToExecute.ExecuteBlock(blockName);

            yield return new WaitForSeconds(5f);

            // See if the execution count is as expected
            int[] executionCounts = new int[sayStates.Count];

            for (int i = 0; i < sayStates.Count; i++)
            {
                var currentState = sayStates[i];
                executionCounts[i] = currentState.ExecutionCount;
            }

            bool savedCorrectly = ExactSameNums(executionCounts, expectedSayExecutionCount);
            Assert.IsTrue(savedCorrectly);
        }


        protected virtual IEnumerator SetUpForEachTest()
        {
            // Try saving the states of the commands involved in this suite; they will have
            // their fields checked to see if they were saved correctly or not
            yield return new WaitForSeconds(0.1f);
            PrepareSayStates();

            SetUpExpectedFields();
        }

        protected virtual void PrepareSayStates()
        {
            List<Say> sayCommands = new List<Say>(GameObject.FindObjectsOfType<Say>());
            sayCommands.Sort((a, b) => a.CommandIndex.CompareTo(b.CommandIndex));
            sayStates = SaySaveUnit.From(sayCommands);
        }

        protected IList<SaySaveUnit> sayStates;

        protected virtual void SetUpExpectedFields()
        {
            expectedSayExecutionCount = new int[sayStates.Count];

            for (int i = 0; i < sayStates.Count; i++)
            {
                expectedSayExecutionCount[i] = 1; // Each Say should be executed just once
            }
        }

        protected int[] expectedSayExecutionCount;

        protected virtual bool ExactSameNums(IList<int> firstList, IList<int> secondList)
        {
            bool differentSizes = firstList.Count != secondList.Count;
            if (differentSizes)
                return false;

            for (int i = 0; i < firstList.Count; i++)
            {
                var firstElem = firstList[i];
                var secondElem = secondList[i];

                if (firstElem != secondElem)
                    return false;
            }

            return true;
        }
    }
}
