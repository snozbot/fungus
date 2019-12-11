using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

/// <summary>
/// It should be no surprise that all of these work, but it is worth confirming there isn't 
/// a silly mistake somewhere
/// </summary>
public class FungusCollectionTests
{
    private Fungus.IntCollection intColA, intColB;
    private Fungus.GameObjectCollection goCol;

    [OneTimeSetUp]
    public void InitTestObjects()
    {
        intColA = new GameObject().AddComponent<Fungus.IntCollection>();
        intColB = new GameObject().AddComponent<Fungus.IntCollection>();
        goCol = new GameObject().AddComponent<Fungus.GameObjectCollection>();
    }

    [OneTimeTearDown]
    public void DestroyTestObjects()
    {
        Object.DestroyImmediate(intColA.gameObject);
        Object.DestroyImmediate(intColB.gameObject);
        Object.DestroyImmediate(goCol.gameObject);
    }

    [Test]
    public void Size()
    {
        intColA.Capacity = 10;

        var startCap = intColA.Capacity;
        var startCount = intColA.Count;

        intColA.Capacity *= 10;
        Assert.Greater(intColA.Capacity, startCap);
        startCap = intColA.Capacity;
        Assert.AreEqual(startCount, intColA.Count);
        intColA.Reserve(intColA.Capacity * 10);
        Assert.Greater(intColA.Capacity, startCap);

        intColA.Resize(startCount + 10);
        Assert.AreEqual(startCount + 10, intColA.Count);

        intColA.Clear();
        intColB.Clear();
    }

    [Test]
    public void GetSet()
    {
        const int ValueA = 1, ValueB = 2;
        intColA.Add(ValueA);
        Assert.AreEqual(ValueA, intColA[0]);
        Assert.AreEqual(ValueA, intColA.Get(0));
        Assert.AreEqual(ValueA, intColA.GetSafe(0));

        intColA.Set(0, ValueB);
        Assert.AreEqual(ValueB, intColA[0]);
        intColA[0] = ValueA;
        Assert.AreEqual(ValueA, intColA[0]);

        intColA.Clear();
        intColB.Clear();
    }

    [Test]
    public void AddRemove()
    {
        //TODO insert
        Assert.NotNull(intColA);
        Assert.NotNull(intColB);

        const int Items = 10;
        const int ItemIndexToRemove = 3;
        const int ValueToRemove = 1;
        const int ValueToAddUnique = ValueToRemove;
        const int ValueToAddDups = ValueToAddUnique;
        const int MinBVal = 7;
        const int SizeAAfterBRem = Items - (Items - MinBVal);

        //add items ensure indicies match
        for (int i = 0; i < Items; i++)
        {
            //added at expected index
            Assert.AreEqual(i, intColA.Add(i));
            //value at index matches expected value
            Assert.AreEqual(i, intColA.Get(i));
        }

        //ensure count matches items added
        Assert.AreEqual(Items, intColA.Count);

        intColA.RemoveAt(ItemIndexToRemove);
        Assert.AreEqual(Items - 1, intColA.Count);

        intColA.Remove(ValueToRemove);
        Assert.AreEqual(Items - 2, intColA.Count);

        //multiple calls to add unique to get back the value we removed and ensure unique works
        for (int i = 0; i < Items; i++)
        {
            intColA.AddUnique(ValueToAddUnique);
        }
        Assert.AreEqual(Items - 1, intColA.Count);

        //now add a bunch of dups, so we can remove all of them and be back at 2 down
        for (int i = 0; i < Items; i++)
        {
            intColA.Add(ValueToAddDups);
        }
        intColA.RemoveAll(ValueToAddDups);
        Assert.AreEqual(Items - 2, intColA.Count);

        //now add a bunch of items to the colB and remove all of them from colA
        for (int i = MinBVal; i < Items; i++)
        {
            intColB.Add(i);
        }
        intColA.RemoveAll(intColB);
        Assert.AreEqual(intColA.Count, SizeAAfterBRem - 2);// all of b should be gone plus the 2 we removed earlier

        intColA.Add(intColB.Get(0));
        intColA.AddUnique(intColB);//ensure intColB[0] doesn't double
        Assert.AreEqual(intColA.Count, Items - 2);// all of b should be back in there now

        intColA.RemoveAll(intColA);//should be equiv to clear
        intColB.Clear();
        Assert.AreEqual(intColA.Count, 0);
        Assert.AreEqual(intColB.Count, 0);

        intColA.Clear();
        intColB.Clear();
    }

    [Test]
    public void Contains()
    {
        //TODO first, last, occurances
        const int Items = 10;
        const int TargetItemValue = 5;
        const int TargetItemIndex = 5;
        const int BItems = 3;

        for (int i = 0; i < Items; i++)
        {
            intColA.Add(i);
        }

        Assert.AreEqual(intColA.IndexOf(TargetItemValue), TargetItemIndex);
        Assert.IsTrue(intColA.Contains(TargetItemValue));

        //any of, put some in colb
        for (int i = 0; i < BItems; i++)
        {
            intColB.Add(i);
        }
        Assert.IsTrue(intColA.ContainsAnyOf(intColB));

        //clear be, put all of in col b
        intColB.Clear();
        intColB.Add(intColA);

        //contain ordered
        Assert.IsTrue(intColA.ContainsAllOfOrdered(intColB));

        //shuffle
        EnsureShuffledDifferent(intColB);

        //contain ordered must fail
        Assert.IsFalse(intColA.ContainsAllOfOrdered(intColB));
        //contain all of must true
        Assert.IsTrue(intColA.ContainsAllOf(intColB));

        intColA.Clear();
        intColB.Clear();
    }

    [Test]
    public void Compat()
    {
        Assert.IsTrue(intColA.IsElementCompatible(7));
        Assert.IsTrue(intColA.IsCollectionCompatible(intColB));
        Assert.IsTrue(intColA.IsCollectionCompatible(new int[] { 1 }));
        Assert.IsTrue(intColA.IsCollectionCompatible(new List<int>()));
        Assert.IsTrue(intColA.IsCollectionCompatible(new List<Fungus.IntegerVariable>()));

        Assert.IsFalse(intColA.IsElementCompatible(Vector3.up));
        Assert.IsFalse(intColA.IsCollectionCompatible(goCol));
        Assert.IsFalse(intColA.IsCollectionCompatible(new Color[] { Color.white }));
        Assert.IsFalse(intColA.IsCollectionCompatible(new List<Material>()));
        Assert.IsFalse(intColA.IsCollectionCompatible(new List<Fungus.StringVariable>()));
    }

    [Test]
    public void Copy()
    {
        const int Items = 10;

        for (int i = 0; i < Items; i++)
        {
            intColA.Add(i);
        }

        intColB.CopyFrom(intColA);

        for (int i = 0; i < intColA.Count; i++)
        {
            Assert.AreEqual(intColA[i], intColB[i]);
        }

        //test enumerator
        var enumer = intColA.GetEnumerator();
        var index = 0;
        while (enumer.MoveNext())
        {
            Assert.AreEqual(enumer.Current, intColB[index++]);
        }

        intColB.Clear();
        intColB.Add(intColA);

        for (int i = 0; i < intColA.Count; i++)
        {
            Assert.AreEqual(intColA[i], intColB[i]);
        }


        intColB.Clear();
        int[] copyDest = new int[intColA.Count];
        intColA.CopyTo(copyDest, 0);

        for (int i = 0; i < intColA.Count; i++)
        {
            Assert.AreEqual(intColA[i], copyDest[i]);
        }

        intColA.Clear();
        intColB.Clear();
    }

    [Test]
    public void Sort()
    {
        const int Items = 10;

        for (int i = 0; i < Items; i++)
        {
            intColA.Add(i);
        }

        //other tests ensure that all actually works
        EnsureShuffledDifferent(intColA);
        intColA.Sort();

        for (int i = 0; i < Items; i++)
        {
            Assert.AreEqual(i, intColA[i]);
        }

        //are they now mirrored
        intColA.Reverse();
        for (int i = 0; i < Items; i++)
        {
            Assert.AreEqual(i, intColA[Items - i - 1]);
        }

        intColA.Clear();
        intColB.Clear();
    }

    [Test]
    public void Exclusive()
    {
        const int Items = 10;
        const int Step = 5;

        for (int i = 0; i < Items; i++)
        {
            intColA.Add(i);
            intColB.Add(i + Step);
        }

        intColA.Exclusive(intColB);

        for (int i = 0; i < intColA.Count; i++)
        {
            Assert.IsTrue(intColA.GetSafe(i) < Step || intColA.GetSafe(i) >= Items);
        }

        Assert.AreEqual(intColA.Count, Items);

        intColA.Clear();
        intColB.Clear();
    }

    [Test]
    public void Intersection()
    {
        const int Items = 10;
        const int Step = 5;

        for (int i = 0; i < Items; i++)
        {
            intColA.Add(i);
            intColB.Add(i + Step);
        }

        intColA.Intersection(intColB);

        for (int i = 0; i < intColA.Count; i++)
        {
            Assert.IsTrue(intColA.GetSafe(i) >= Step || intColA.GetSafe(i) < Items);
        }

        Assert.AreEqual(intColA.Count, Step);

        intColA.Clear();
        intColB.Clear();
    }

    private void EnsureShuffledDifferent(Fungus.IntCollection col)
    {
        var startval = col.GetSafe(0);
        col.Shuffle();
        //don't let shuffle result in the same seq
        if (col.GetSafe(0) == startval)
        {
            col[0] = col[col.Count - 1];
            col[col.Count - 1] = startval;
        }
    }
}
