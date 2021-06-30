// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using NUnit.Framework;

namespace Fungus.Tests
{
    [TestFixture]
    public class TextVariationSelectionTests
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
            if (res != startingTextSubA && res != startingTextSubB)
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

        [Test]
        public void SquareBracketsWithoutTypeNoImpact()
        {
            Fungus.TextVariationHandler.ClearHistory();

            string startingText = @"This is test a [of changing nothing]";
            const string expected = @"This is test a [of changing nothing]";

            string res = string.Empty;

            res = Fungus.TextVariationHandler.SelectVariations(startingText);

            Assert.AreEqual(expected, res);
        }
    }
}