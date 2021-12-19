using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine; 
using UnityEngine.TestTools;
using Fungus.LionManeSaveSys;
using Fungus;

namespace SaveSystemTests
{
    public class FlowchartBlockSavingTests : SaveSysPlayModeTest
    {
        protected override string PathToScene => "Prefabs/FlowchartSavingTests";

        public override void SetUp()
        {
            base.SetUp();
            blockSaver = GameObject.FindObjectOfType<BlockSaver>();
            FindBlockToSave();
            SetUpExpectedFields();
        }

        protected virtual void FindBlockToSave()
        {
            var allBlocks = GameObject.FindObjectsOfType<Block>();
            foreach (var blockFound in allBlocks)
            {
                if (blockFound.BlockName == nameOfBlockToSave)
                {
                    blockToSave = blockFound;
                    break;
                }
            }
        }

        protected BlockSaver blockSaver;
        protected string nameOfBlockToSave = "Dialogue";
        protected Block blockToSave;

        protected virtual void SetUpExpectedFields()
        {
            expectedBlockItemId = blockToSave.ItemId;
        }
        
        protected int dialogueBlockIndex = 3, menuBlockIndex = 2, expectedBlockItemId;
        protected int[] expectedSayIndexes = new int[] // within the Dialogue block
        {
            3, 4
        };

        [UnityTest]
        public virtual IEnumerator CommandIndexesSaved()
        {
            yield return PostSetUp();
            
            List<Say> sayCommands = new List<Say>(blockToSave.GetComponents<Say>());
            sayCommands.Sort(SortByIndex);

            IList<int> commandIndexesFound = GetSayCommandIndexesOf(blockState);

            bool savedCorrectly = SameIntsInSameOrder(commandIndexesFound, expectedSayIndexes);
            Assert.IsTrue(savedCorrectly);
        }

        protected virtual IList<int> GetSayCommandIndexesOf(BlockSaveUnit blockState)
        {
            List<int> indexes = new List<int>();

            foreach (var commandState in blockState.Commands)
            {
                if (commandState.TypeName == "SayCommand")
                    indexes.Add(commandState.Index);
            }

            return indexes;
        }

        protected virtual int SortByIndex(Say firstSay, Say secondSay)
        {
            if (firstSay.CommandIndex > secondSay.CommandIndex)
                return 1;
            else
                return -1;
        }

        protected virtual IList<int> CommandIndexesOf(IList<Say> sayCommands)
        {
            int[] indexes = new int[sayCommands.Count];

            for (int i = 0; i < sayCommands.Count; i++)
            {
                Say currentCommand = sayCommands[i];
                indexes[i] = currentCommand.CommandIndex;
            }

            return indexes;
        }

        protected virtual bool SameIntsInSameOrder(IList<int> firstArr, IList<int> secondArr)
        {
            if (firstArr.Count != secondArr.Count)
                return false;

            for (int i = 0; i < firstArr.Count; i++)
            {
                int firstNum = firstArr[i];
                int secondNum = secondArr[i];
                if (firstNum != secondNum)
                    return false;
            }

            return true;
        }

        protected virtual IEnumerator PostSetUp()
        {
            yield return new WaitForSeconds(1f);
            blockToSave.SetExecutionCount(expectedExecutionCount);
            SaveTheTestBlock();

        }

        int expectedExecutionCount = 8192;

        protected virtual void SaveTheTestBlock()
        {
            blockState = blockSaver.CreateSaveFrom(blockToSave);
        }

        protected BlockSaveUnit blockState;

        [UnityTest]
        public virtual IEnumerator ExecutionCountSaved()
        {
            yield return PostSetUp();

            bool savedCorrectly = blockState.ExecutionCount == expectedExecutionCount;
            Assert.IsTrue(savedCorrectly);
        }

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator SayCommandsSaved()
        {
            throw new System.NotImplementedException();
        }

        [UnityTest]
        public virtual IEnumerator ItemIDsSaved()
        {
            yield return PostSetUp();

            bool savedCorrectly = blockState.ItemId == expectedBlockItemId;
            Assert.IsTrue(savedCorrectly);
        }
    }
}
