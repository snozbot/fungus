using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace SaveSystemTests
{
    public class FlowchartCommandSavingTests : SaveSysPlayModeTest
    {
        public override void SetUp()
        {
            base.SetUp();
            SetUpExpectedFields();
        }

        protected virtual void SetUpExpectedFields()
        {

        }

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator MenuHideIfVisitedSaved()
        {
            // Might want to erase this test later on... The Menu Command tells whether its target block was
            // visited based on said target's execution count rather than some other flag in said Command
            throw new System.NotImplementedException();
        }
    }
}
