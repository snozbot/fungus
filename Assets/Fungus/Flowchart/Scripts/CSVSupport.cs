using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace Fungus
{

	// Some CSV utilities cobbled together from stack overflow answers
	// CSV escape & unescape from http://stackoverflow.com/questions/769621/dealing-with-commas-in-a-csv-file
	// http://answers.unity3d.com/questions/144200/are-there-any-csv-reader-for-unity3d-without-needi.html
	public static class CSVSupport
	{
		public static string Escape( string s )
		{
			s = s.Replace("\n", "\\n");
			
			if ( s.Contains( QUOTE ) )
				s = s.Replace( QUOTE, ESCAPED_QUOTE );
			
			if ( s.IndexOfAny( CHARACTERS_THAT_MUST_BE_QUOTED ) > -1 )
				s = QUOTE + s + QUOTE;
			
			return s;
		}
		
		public static string Unescape( string s )
		{
			s = s.Replace("\\n", "\n");

			if ( s.StartsWith( QUOTE ) && s.EndsWith( QUOTE ) )
			{
				s = s.Substring( 1, s.Length - 2 );
				
				if ( s.Contains( ESCAPED_QUOTE ) )
					s = s.Replace( ESCAPED_QUOTE, QUOTE );
			}
			
			return s;
		}
				
		private const string QUOTE = "\"";
		private const string ESCAPED_QUOTE = "\"\"";
		private static char[] CHARACTERS_THAT_MUST_BE_QUOTED = { ',', '"', '\n' };
	}
	
}
