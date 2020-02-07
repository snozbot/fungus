// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using NUnit.Framework;
using System.Collections.Generic;

public class FungusConversationParseTests
{
    private const string SimpleConv =
@"john bored left: Oh, so that's how you use the Conversation command.
sherlock eyeroll right nowait: Yes, well done John. {w=1.5}
You catch on quickly don't you?
sherlock >>>:
hide john ""offscreen left"": I sure do.

-- This is a comment, it doesn't appear in the conversation
";
    private static readonly List<Fungus.ConversationManager.RawConversationItem> SimpleConvRes = new List<Fungus.ConversationManager.RawConversationItem>()
    {
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ "john", "bored", "left" }, text = "Oh, so that's how you use the Conversation command."},
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ "sherlock", "eyeroll", "right", "nowait" }, text = "Yes, well done John. {w=1.5}"},
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ }, text = "You catch on quickly don't you?"},
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ "sherlock", ">>>" }, text = ""},
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ "hide", "john", "offscreen left" }, text = "I sure do."},
    };

    [Test]
    public void FungusConversationPreParseSimple()
    {
        var res = Fungus.ConversationManager.PreParse(SimpleConv);
        ValueCompareRawConversationItemLists(res, SimpleConvRes);
    }

    private void ValueCompareRawConversationItemLists(List<Fungus.ConversationManager.RawConversationItem> lhs, List<Fungus.ConversationManager.RawConversationItem> rhs)
    {
        Assert.AreEqual(lhs.Count, rhs.Count, "Different number of results found.");

        for (int i = 0; i < lhs.Count; i++)
        {
            Assert.AreEqual(lhs[i].text, rhs[i].text, "RawConItem " + i.ToString() + " has different text body.");
            Assert.AreEqual(lhs[i].sayParams.Length, rhs[i].sayParams.Length, "RawConItem " + i.ToString() + " have differing say param counts");
            for (int j = 0; j < lhs[i].sayParams.Length; j++)
            {
                Assert.AreEqual(lhs[i].sayParams[j], rhs[i].sayParams[j], "RawConItem " + i.ToString() + " param: " + j.ToString() + " content");
            }
        }
    }

    private const string MultiColonConv =
@"sherlock left: Outragous!
john bashful: This is an overreaction Sherlock.
sherlock: Colon to direct attention to a list. Colon to emphasize connecton between independent phrases.
: To Buy: Eggs, Milk, Flour.
sherlock irate right: In this world, there are only two tragedies: one is not getting what one wants, and the other is getting it.
john smug:Love is blind: sometimes it keeps us from seeing the truth.";
    private static readonly List<Fungus.ConversationManager.RawConversationItem> MultiColonConvRes = new List<Fungus.ConversationManager.RawConversationItem>()
    {
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ "sherlock", "left" }, text = "Outragous!"},
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ "john", "bashful" }, text = "This is an overreaction Sherlock."},
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ "sherlock" }, text = "Colon to direct attention to a list. Colon to emphasize connecton between independent phrases."},
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ }, text = "To Buy: Eggs, Milk, Flour."},
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ "sherlock", "irate", "right" }, text = "In this world, there are only two tragedies: one is not getting what one wants, and the other is getting it."},
        new Fungus.ConversationManager.RawConversationItem(){ sayParams = new string[]{ "john", "smug" }, text = "Love is blind: sometimes it keeps us from seeing the truth."},
    };

    [Test]
    public void FungusConversationPreParseMultiColon()
    {
        var res = Fungus.ConversationManager.PreParse(MultiColonConv);
        ValueCompareRawConversationItemLists(res, MultiColonConvRes);
    }
}
