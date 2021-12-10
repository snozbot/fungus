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
        public virtual IEnumerator SayExecutionCountSaved()
        {
            yield return SetUpForEachTest();

            // For some reason, Say Commands don't execute properly in unit-testing environments, so we
            // have to update the execution counts manually here instead of executing the Blocks 
            // said Say Commands are in...
            var allSayCommands = GameObject.FindObjectsOfType<Say>();

            foreach (var sayCommand in allSayCommands)
            {
                sayCommand.ExecutionCount = 1;
            }

            PrepareSayStates(); // Since we need said states updated

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
            PrepareBlockStates();

            SetUpExpectedFields();
        }

        protected virtual void PrepareSayStates()
        {
            List<Say> sayCommands = new List<Say>(GameObject.FindObjectsOfType<Say>());
            sayCommands.Sort((firstSay, secondSay) => firstSay.CommandIndex.CompareTo(secondSay.CommandIndex));
            sayStates = SaySaveUnit.From(sayCommands);
        }

        protected IList<SaySaveUnit> sayStates;

        protected virtual void PrepareBlockStates()
        {
            List<Block> blocks = new List<Block>(GameObject.FindObjectsOfType<Block>());
            blocks.Sort((firstBlock, secondBlock) => firstBlock.BlockName.CompareTo(secondBlock.BlockName));
            blockStates = BlockSaveUnit.From(blocks);
        }

        protected IList<BlockSaveUnit> blockStates;

        protected virtual void SetUpExpectedFields()
        {
            expectedSayExecutionCount = new int[sayStates.Count];

            for (int i = 0; i < sayStates.Count; i++)
            {
                expectedSayExecutionCount[i] = 1; // Each Say should be executed just once
            }

            expectedBlockExecutionCount = new int[blockStates.Count];

            for (int i = 0; i < blockStates.Count; i++)
            {
                expectedBlockExecutionCount[i] = 2;
            }
        }

        protected int[] expectedSayExecutionCount, expectedBlockExecutionCount;

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

        [UnityTest]
        public virtual IEnumerator BlockExecutionCountsSaved()
        {
            yield return SetUpForEachTest();

            var allBlocks = GameObject.FindObjectsOfType<Block>();

            foreach (var block in allBlocks)
            {
                block.SetExecutionCount(2);
            }

            PrepareBlockStates();

            int[] executionCounts = new int[blockStates.Count];

            for (int i = 0; i < blockStates.Count; i++)
            {
                var currentState = blockStates[i];
                executionCounts[i] = currentState.ExecutionCount;
            }

            bool savedCorrectly = ExactSameNums(executionCounts, expectedBlockExecutionCount);
            Assert.IsTrue(savedCorrectly);
        }
    }
}
