using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine; 
using UnityEngine.TestTools; 
using Fungus; 

namespace SaveSystemTests
{
    public class FlowchartBlockSavingTests : SaveSysPlayModeTest
    {
        public override void SetUp()
        {
            base.SetUp();

        }

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator CommandIndexesSaved()
        {
            throw new System.NotImplementedException();
        }

        [UnityTest]
        [Ignore("")]
        public virtual IEnumerator CommandTypeSaved()
        {
            throw new System.NotImplementedException();
        }
    }
}
