using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus
{
	public enum TokenType
	{
		Words,					// A string of words
		BoldStart,				// b
		BoldEnd,				// /b
		ItalicStart,			// i
		ItalicEnd,				// /i
		ColorStart,				// color=red
		ColorEnd,				// /color
		Wait, 					// w, w=0.5
		WaitForInputNoClear, 	// wi
		WaitForInputAndClear, 	// wc
		WaitOnPunctuationStart, // wp, wp=0.5
		WaitOnPunctuationEnd,   // /wp
		Clear, 					// c
		SpeedStart, 			// s, s=60
		SpeedEnd, 				// /s
		Exit, 					// x
		Message,				// m=MessageName
		VerticalPunch,       	// {vpunch=0.5}
		HorizontalPunch,        // {hpunch=0.5}
		Shake,					// {shake=0.5}
		Shiver,                 // {shiver=0.5}
		Flash,					// {flash=0.5}
		Audio,					// {audio=Sound}
		AudioLoop,				// {audioloop=Sound}
		AudioPause,				// {audiopause=Sound}
		AudioStop				// {audiostop=Sound}
	}
	
	public class Token
	{
		public TokenType type = TokenType.Words;
		public string param = "";
	}

	public class DialogParser
	{
		public List<Token> tokens = new List<Token>();

		public virtual void Tokenize(string storyText)
		{
			tokens.Clear();

			string pattern = @"\{.*?\}";
			Regex myRegex = new Regex(pattern);
			
			Match m = myRegex.Match(storyText);   // m is the first match
			
			int position = 0;
			while (m.Success)
			{
				// Get bit leading up to tag
				string preText = storyText.Substring(position, m.Index - position);
				string tagText = m.Value;
				
				AddWordsToken(tokens, preText);
				AddTagToken(tokens, tagText);
				
				position = m.Index + tagText.Length;
				m = m.NextMatch();
			}

			if (position < storyText.Length)
			{
				string postText = storyText.Substring(position, storyText.Length - position);
				if (postText.Length > 0)
				{
					AddWordsToken(tokens, postText);
				}
			}

			// Remove all leading whitespace & newlines after a {c} or {wc} tag
			// These characters are usually added for legibility when editing, but are not 
			// desireable when viewing the text in game.
			bool trimLeading = false;
			foreach (Token token in tokens)
			{
				if (trimLeading &&
				    token.type == TokenType.Words)
				{
					token.param.TrimStart(' ', '\t', '\r', '\n');
				}

				if (token.type == TokenType.Clear || 
				    token.type == TokenType.WaitForInputAndClear)
				{
					trimLeading = true;
				}
				else
				{
					trimLeading = false;
				}
			}
		}
		
		protected static void AddWordsToken(List<Token> tokenList, string words)
		{
			Token token = new Token();
			token.type = TokenType.Words;
			token.param = words;
			tokenList.Add(token);
		}
		
		protected virtual void AddTagToken(List<Token> tokenList, string tagText)
		{
			if (tagText.Length < 3 ||
			    tagText.Substring(0,1) != "{" ||
			    tagText.Substring(tagText.Length - 1,1) != "}")
			{
				return;
			}
			
			string tag = tagText.Substring(1, tagText.Length - 2);
			
			TokenType type = TokenType.Words;
			string paramText = "";
			
			if (tag == "b")
			{
				type = TokenType.BoldStart;
			}
			else if (tag == "/b")
			{
				type = TokenType.BoldEnd;
			}
			else if (tag == "i")
			{
				type = TokenType.ItalicStart;
			}
			else if (tag == "/i")
			{
				type = TokenType.ItalicEnd;
			}
			else if (tag.StartsWith("color="))
			{
				type = TokenType.ColorStart;
				paramText = tag.Substring(6, tag.Length - 6);
			}
			else if (tag == "/color")
			{
				type = TokenType.ColorEnd;
			}
			else if (tag == "wi")
			{
				type = TokenType.WaitForInputNoClear;
			}
			if (tag == "wc")
			{
				type = TokenType.WaitForInputAndClear;
			}
			else if (tag.StartsWith("wp="))
			{
				type = TokenType.WaitOnPunctuationStart;
				paramText = tag.Substring(3, tag.Length - 3);
			}
			else if (tag == "wp")
			{
				type = TokenType.WaitOnPunctuationStart;
			}
			else if (tag == "/wp")
			{
				type = TokenType.WaitOnPunctuationEnd;
			}
			else if (tag.StartsWith("w="))
			{
				type = TokenType.Wait;
				paramText = tag.Substring(2, tag.Length - 2);
			}
			else if (tag == "w")
			{
				type = TokenType.Wait;
			}
			else if (tag == "c")
			{
				type = TokenType.Clear;
			}
			else if (tag.StartsWith("s="))
			{
				type = TokenType.SpeedStart;
				paramText = tag.Substring(2, tag.Length - 2);
			}
			else if (tag == "s")
			{
				type = TokenType.SpeedStart;
			}
			else if (tag == "/s")
			{
				type = TokenType.SpeedEnd;
			}
			else if (tag == "x")
			{
				type = TokenType.Exit;
			}
			else if (tag.StartsWith("m="))
			{
				type = TokenType.Message;
				paramText = tag.Substring(2, tag.Length - 2);
			}
			else if (tag.StartsWith("vpunch="))
			{
				type = TokenType.VerticalPunch;
				paramText = tag.Substring(7, tag.Length - 7);
			}
			else if (tag.StartsWith("hpunch="))
			{
				type = TokenType.HorizontalPunch;
				paramText = tag.Substring(7, tag.Length - 7);
			}
			else if (tag.StartsWith("shake="))
			{
				type = TokenType.Shake;
				paramText = tag.Substring(6, tag.Length - 6);
			}
			else if (tag.StartsWith("shiver="))
			{
				type = TokenType.Shiver;
				paramText = tag.Substring(7, tag.Length - 7);
			}
			else if (tag.StartsWith("flash="))
			{
				type = TokenType.Flash;
				paramText = tag.Substring(6, tag.Length - 6);
			}
			else if (tag.StartsWith("audio="))
			{
				type = TokenType.Audio;
				paramText = tag.Substring(6, tag.Length - 6);
			}
			else if (tag.StartsWith("audioloop="))
			{
				type = TokenType.AudioLoop;
				paramText = tag.Substring(10, tag.Length - 10);
			}
			Token token = new Token();
			token.type = type;
			token.param = paramText.Trim();
			
			tokenList.Add(token);
		}
	}

}