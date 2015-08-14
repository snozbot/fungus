using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

using Fungus;

[TestFixture]
public class TextTagParserTests
{
	[Test]
	public void TextTagParser_Parser()
	{
		var textTagParser = new TextTagParser();

		// Parse an example string, generate correct sequence of tags
		List<TextTagParser.Token> tokens = textTagParser.Tokenize("Words " + 
		                                                          "{b}bold test{/b}" +
		                                                          "{i}italic test{/i}" +
		                                                          "{color=red}color test{/color}" +
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
		Assert.That(tokens[i].type == TextTagParser.TokenType.Words);
		Assert.That(tokens[i].param == "Words ");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.BoldStart);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Words);
		Assert.That(tokens[i].param == "bold test");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.BoldEnd);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.ItalicStart);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Words);
		Assert.That(tokens[i].param == "italic test");
		
		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.ItalicEnd);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.ColorStart);
		Assert.That(tokens[i].param == "red");
		
		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Words);
		Assert.That(tokens[i].param == "color test");
		
		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.ColorEnd);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Wait);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Wait);
		Assert.That(tokens[i].param == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.WaitForInputNoClear);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.WaitForInputAndClear);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.WaitOnPunctuationStart);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.WaitOnPunctuationStart);
		Assert.That(tokens[i].param == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.WaitOnPunctuationEnd);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Clear);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.SpeedStart);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.SpeedStart);
		Assert.That(tokens[i].param == "60");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.SpeedEnd);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Exit);
		Assert.That(tokens[i].param == "");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Message);
		Assert.That(tokens[i].param == "Message");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.VerticalPunch);
		Assert.That(tokens[i].param == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.HorizontalPunch);
		Assert.That(tokens[i].param == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Punch);
		Assert.That(tokens[i].param == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Flash);
		Assert.That(tokens[i].param == "0.5");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Audio);
		Assert.That(tokens[i].param == "Sound");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.AudioLoop);
		Assert.That(tokens[i].param == "Sound");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.AudioPause);
		Assert.That(tokens[i].param == "Sound");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.AudioStop);
		Assert.That(tokens[i].param == "Sound");

		Assert.That(tokens.Count == 31);
	}

	[Test]
	public void TextTagParser_AudioWaitBug()
	{
		var textTagParser = new TextTagParser();
		
		// Parse an example string, generate correct sequence of tags
		List<TextTagParser.Token> tokens = textTagParser.Tokenize("Play sound{audio=BeepSound}{w=1} Play loop{audioloop=BeepSound}{w=3} Stop{audiostop=BeepSound}");
		
		int i = 0;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Words);
		Assert.That(tokens[i].param == "Play sound");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Audio);
		Assert.That(tokens[i].param == "BeepSound");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Wait);
		Assert.That(tokens[i].param == "1");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Words);
		Assert.That(tokens[i].param == " Play loop");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.AudioLoop);
		Assert.That(tokens[i].param == "BeepSound");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Wait);
		Assert.That(tokens[i].param == "3");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.Words);
		Assert.That(tokens[i].param == " Stop");

		i++;
		Assert.That(tokens[i].type == TextTagParser.TokenType.AudioStop);
		Assert.That(tokens[i].param == "BeepSound");

		Assert.That(tokens.Count == 8);
	}
	
}
