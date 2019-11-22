using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class FungusTextVariationSelectionTests
{
    [Test]
    public void SimpleSequenceSelection()
    {
        Fungus.TextVariationHandler.ClearHistory();

        string startingText = @"This is test [a|b|c]";
        string startingTextA = @"This is test a";
        string startingTextB = @"This is test b";
        string startingTextC = @"This is test c";

        string res = string.Empty;

        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextA);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextB);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextC);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextC);
    }

    [Test]
    public void SimpleCycleSelection()
    {
        Fungus.TextVariationHandler.ClearHistory();

        string startingText = @"This is test [&a|b|c]";
        string startingTextA = @"This is test a";
        string startingTextB = @"This is test b";
        string startingTextC = @"This is test c";

        string res = string.Empty;

        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextA);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextB);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextC);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextA);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextB);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextC);
    }

    [Test]
    public void SimpleOnceSelection()
    {
        Fungus.TextVariationHandler.ClearHistory();

        string startingText = @"This is test [!a|b|c]";
        string startingTextA = @"This is test a";
        string startingTextB = @"This is test b";
        string startingTextC = @"This is test c";
        string startingTextD = @"This is test ";

        string res = string.Empty;

        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextA);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextB);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextC);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextD);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextD);
    }

    [Test]
    public void NestedSelection()
    {
        Fungus.TextVariationHandler.ClearHistory();

        string startingText = @"This is test [a||sub [~a|b]|[!b|[~c|d]]]";
        string startingTextA = @"This is test a";
        string startingTextBlank = @"This is test ";
        string startingTextSubA = @"This is test sub a";
        string startingTextSubB = @"This is test sub b";
        string startingTextB = @"This is test b";
        string startingTextC = @"This is test c";
        string startingTextD = @"This is test d";

        string res = string.Empty;

        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextA);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextBlank);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        if(res != startingTextSubA && res != startingTextSubB)
        {
            Assert.Fail();
        }
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        Assert.AreEqual(res, startingTextB);
        res = Fungus.TextVariationHandler.SelectVariations(startingText);
        if (res != startingTextC && res != startingTextD)
        {
            Assert.Fail();
        }
    }
}
