using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class FungusPrioritySignalsTest {

    private int changeCallCount, startCallCount, endCallCount;

	[Test]
	public void CountsAndSignals()
    {
        Fungus.FungusPrioritySignals.OnFungusPriorityStart += FungusPrioritySignals_OnFungusPriorityStart;
        Fungus.FungusPrioritySignals.OnFungusPriorityEnd += FungusPrioritySignals_OnFungusPriorityEnd;
        Fungus.FungusPrioritySignals.OnFungusPriorityChange += FungusPrioritySignals_OnFungusPriorityChange;

        Assert.Zero(Fungus.FungusPrioritySignals.CurrentPriorityDepth);

        Fungus.FungusPrioritySignals.DoIncreasePriorityDepth();
        //one start, one change, no end, 1 depth
        Assert.AreEqual(0, endCallCount);
        Assert.AreEqual(1, startCallCount);
        Assert.AreEqual(1, changeCallCount);
        Assert.AreEqual(1, Fungus.FungusPrioritySignals.CurrentPriorityDepth);



        Fungus.FungusPrioritySignals.DoIncreasePriorityDepth();
        //one start, 2 change, no end, 2 depth
        Assert.AreEqual(0, endCallCount);
        Assert.AreEqual(1, startCallCount);
        Assert.AreEqual(2, changeCallCount);
        Assert.AreEqual(2, Fungus.FungusPrioritySignals.CurrentPriorityDepth);


        Fungus.FungusPrioritySignals.DoIncreasePriorityDepth();
        //one start, 3 change, no end, 3 depth
        Assert.AreEqual(0, endCallCount);
        Assert.AreEqual(1, startCallCount);
        Assert.AreEqual(3, changeCallCount);
        Assert.AreEqual(3, Fungus.FungusPrioritySignals.CurrentPriorityDepth);


        Fungus.FungusPrioritySignals.DoDecreasePriorityDepth();
        //one start, 4 change, no end, 2 depth
        Assert.AreEqual(0, endCallCount);
        Assert.AreEqual(1, startCallCount);
        Assert.AreEqual(4, changeCallCount);
        Assert.AreEqual(2, Fungus.FungusPrioritySignals.CurrentPriorityDepth);


        Fungus.FungusPrioritySignals.DoDecreasePriorityDepth();
        Fungus.FungusPrioritySignals.DoDecreasePriorityDepth();
        //one start, 6 change, 1 end, 0 depth
        Assert.AreEqual(1, endCallCount);
        Assert.AreEqual(1, startCallCount);
        Assert.AreEqual(6, changeCallCount);
        Assert.AreEqual(0, Fungus.FungusPrioritySignals.CurrentPriorityDepth);

        Fungus.FungusPrioritySignals.OnFungusPriorityStart -= FungusPrioritySignals_OnFungusPriorityStart;
        Fungus.FungusPrioritySignals.OnFungusPriorityEnd -= FungusPrioritySignals_OnFungusPriorityEnd;
        Fungus.FungusPrioritySignals.OnFungusPriorityChange -= FungusPrioritySignals_OnFungusPriorityChange;

        //unsubbed so all the same
        Fungus.FungusPrioritySignals.DoIncreasePriorityDepth();
        Fungus.FungusPrioritySignals.DoDecreasePriorityDepth();
        //one start, 6 change, 1 end, 0 depth
        Assert.AreEqual(1, endCallCount);
        Assert.AreEqual(1, startCallCount);
        Assert.AreEqual(6, changeCallCount);
        Assert.AreEqual(0, Fungus.FungusPrioritySignals.CurrentPriorityDepth);
    }

    private void FungusPrioritySignals_OnFungusPriorityChange(int previousActiveDepth, int newActiveDepth)
    {
        changeCallCount++;
    }

    private void FungusPrioritySignals_OnFungusPriorityEnd()
    {
        endCallCount++;
    }

    private void FungusPrioritySignals_OnFungusPriorityStart()
    {
        startCallCount++;
    }
}
