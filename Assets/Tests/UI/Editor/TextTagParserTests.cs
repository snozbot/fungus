// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using Fungus;

#if UNITY_5_3_OR_NEWER

using NUnit.Framework;


[TestFixture]
public class TextTagParserTests
{
	[Test]
	public void TextTagParser_Parser()
	{
		var textTagParser = new TextTagParser();

		// Parse an example string, generate correct sequence of tags
		List<TextTagToken> tokens = textTagParser.Tokenize("Words " + 
		                                                          "{b}bold test{/b}" +
		                                                          "{i}italic test{/i}" +
		                                                          "{color=red}color test{/color}" +
																  "{size=30}size test{/size}" +
		                                                          "{w}{w=0.5}" +
		                                                          "{wi}{wc}" +
		                                                          "{wp}{wp=0.5}{/wp}" +
		                                                          "{c}" +
		                                                          "{s}{s=60}{/s}" +
		                                                          "{x}{m=Message}" +
		                                                          "{vpunch=0.5}" +
		                                                          "{hpunch=0.5}" +
		                                                          "{punch=0.5}" +
		                                                          "{flash=0.5}" +
		                                                          "{audio=Sound}" +
		                                                          "{audioloop=Sound}" +
		                                                          "{audiopause=Sound}" +
		                                                          "{audiostop=Sound}");

		int i = 0;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Words);
		Assert.That(tokens[i].paramList[0] == "Words ");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.BoldStart);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Words);
		Assert.That(tokens[i].paramList[0] == "bold test");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.BoldEnd);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.ItalicStart);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Words);
		Assert.That(tokens[i].paramList[0] == "italic test");
		
		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.ItalicEnd);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.ColorStart);
		Assert.That(tokens[i].paramList[0] == "red");
		
		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Words);
		Assert.That(tokens[i].paramList[0] == "color test");
		
		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.ColorEnd);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.SizeStart);
		Assert.That(tokens[i].paramList[0] == "30");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Words);
		Assert.That(tokens[i].paramList[0] == "size test");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.SizeEnd);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Wait);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Wait);
		Assert.That(tokens[i].paramList[0] == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.WaitForInputNoClear);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.WaitForInputAndClear);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.WaitOnPunctuationStart);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.WaitOnPunctuationStart);
		Assert.That(tokens[i].paramList[0] == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.WaitOnPunctuationEnd);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Clear);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.SpeedStart);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.SpeedStart);
		Assert.That(tokens[i].paramList[0] == "60");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.SpeedEnd);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Exit);
		Assert.That(tokens[i].paramList.Count == 0);

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Message);
		Assert.That(tokens[i].paramList[0] == "Message");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.VerticalPunch);
		Assert.That(tokens[i].paramList[0] == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.HorizontalPunch);
		Assert.That(tokens[i].paramList[0] == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Punch);
		Assert.That(tokens[i].paramList[0] == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Flash);
		Assert.That(tokens[i].paramList[0] == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Audio);
		Assert.That(tokens[i].paramList[0] == "Sound");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.AudioLoop);
		Assert.That(tokens[i].paramList[0] == "Sound");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.AudioPause);
		Assert.That(tokens[i].paramList[0] == "Sound");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.AudioStop);
		Assert.That(tokens[i].paramList[0] == "Sound");

		Assert.That(tokens.Count == 34);
	}

	[Test]
	public void TextTagParser_AudioWaitBug()
	{
		var textTagParser = new TextTagParser();
		
		// Parse an example string, generate correct sequence of tags
		List<TextTagToken> tokens = textTagParser.Tokenize("Play sound{audio=BeepSound}{w=1} Play loop{audioloop=BeepSound}{w=3} Stop{audiostop=BeepSound}");
		
		int i = 0;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Words);
		Assert.That(tokens[i].paramList[0] == "Play sound");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Audio);
		Assert.That(tokens[i].paramList[0] == "BeepSound");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Wait);
		Assert.That(tokens[i].paramList[0] == "1");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Words);
		Assert.That(tokens[i].paramList[0] == " Play loop");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.AudioLoop);
		Assert.That(tokens[i].paramList[0] == "BeepSound");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Wait);
		Assert.That(tokens[i].paramList[0] == "3");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.Words);
		Assert.That(tokens[i].paramList[0] == " Stop");

		i++;
		Assert.That(tokens[i].type == TextTagToken.TokenType.AudioStop);
		Assert.That(tokens[i].paramList[0] == "BeepSound");

		Assert.That(tokens.Count == 8);
	}
	
}

#endif
