using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Fungus;

namespace SaveSystemTests
{
    public abstract class FlowchartSavingTests: SaveSysPlayModeTest
    {
        protected override string PathToScene => "Prefabs/FlowchartSavingTests";
        protected Flowchart[] allFlowcharts;

        public override void SetUp()
        {
            base.SetUp();
            allFlowcharts = GameObject.FindObjectsOfType<Flowchart>();
            flowchartEncodersGO = GameObject.Find(flowchartEncodersGOName);

        }

        protected GameObject flowchartEncodersGO;
        protected string flowchartEncodersGOName = "FlowchartEncoders";

    }
}
