//Author:	Richard Lopes (2002)
//Author:	Marc Kohaupt (2008-2009) [aliases]
//License:	MIT License
//URL:		http://www.codeproject.com/KB/recipes/command_line.aspx

using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace CommandLine.Utility
{
	/// <summary>
	/// Klasse, um Kommandozeile-Parameter zu verarbeiten.
	/// Dabei wird Unix, als auch Windows-Style unterstützt.
	/// Ebenso werden Argumente mit Wert als auch ohne Wert unterstützt
	/// (letztere sind dann boolsch gesetzt oder nicht gesetzt)
	/// 
	/// Beispiele:
	/// -param1 value1
	/// --param2 value2
	/// /param3:"Test-:-work"
	/// /param4=happy
	/// -param5 '--=nice=--'
	/// </summary>
	/// <description>
	/// Arguments clArgs = new Arguments(args);
	/// 
	/// clArgs.AddAlias("help", "h");
    /// 
    /// if (clArgs["help"] != null)
    /// {
    ///    ...
    /// }
	/// </description>
	
	public class Arguments
	{
		// Variables
		private StringDictionary Parameters;
		
		// Constructor
		public Arguments(string[] Args)
		{
			Parameters = new StringDictionary();
			Regex Spliter = new Regex(@"^-{1,2}|^/|=|:",
			                          RegexOptions.IgnoreCase|RegexOptions.Compiled);
			
			Regex Remover = new Regex(@"^['""]?(.*?)['""]?$",
			                          RegexOptions.IgnoreCase|RegexOptions.Compiled);
			
			string Parameter = null;
			string[] Parts;
			
			// Valid parameters forms:
			// {-,/,--}param{ ,=,:}((",')value(",'))
			// Examples: 
			// -param1 value1 --param2 /param3:"Test-:-work" 
			//   /param4=happy -param5 '--=nice=--'
			
			foreach(string Txt in Args)
			{
				// Look for new parameters (-,/ or --) and a
				// possible enclosed value (=,:)
				
				Parts = Spliter.Split(Txt,3);
				
				switch(Parts.Length){
					// Found a value (for the last parameter 
					// found (space separator))
				case 1:
					if(Parameter != null)
					{
						if(!Parameters.ContainsKey(Parameter)) 
						{
							Parts[0] = Remover.Replace(Parts[0], "$1");
							
							Parameters.Add(Parameter, Parts[0]);
						}
						Parameter=null;
					}
					// else Error: no parameter waiting for a value (skipped)
					
					break;

					// Found just a parameter
				case 2:
					// The last parameter is still waiting. 
					// With no value, set it to true.
					if(Parameter!=null)
					{
						if(!Parameters.ContainsKey(Parameter)) 
							Parameters.Add(Parameter, "true");
					}
					Parameter=Parts[1];
					break;
					
					// Parameter with enclosed value
				case 3:
					// The last parameter is still waiting. 
					// With no value, set it to true.
					if(Parameter != null)
					{
						if(!Parameters.ContainsKey(Parameter)) 
							Parameters.Add(Parameter, "true");
					}
					
					Parameter = Parts[1];
					
					// Remove possible enclosing characters (",')
					if(!Parameters.ContainsKey(Parameter))
					{
						Parts[2] = Remover.Replace(Parts[2], "$1");
						Parameters.Add(Parameter, Parts[2]);
					}
					
					Parameter=null;
					break;
				}
			}
			// In case a parameter is still waiting
			if(Parameter != null)
			{
				if(!Parameters.ContainsKey(Parameter)) 
					Parameters.Add(Parameter, "true");
			}
		}
		
		// Retrieve a parameter value if it exists 
		// (overriding C# indexer property)
		public string this [string Param]
		{
			get
			{
				return(Parameters[Param]);
			}
		}
		
		/// <summary>
		/// Fügt einer Option einen Alias hinzu.
		/// So kann z.B. statt der Langform /help auch die Kurzform /h beim Programmaufruf benutzt werden.
		/// (Im Programm selber sollte jedoch der 'eigentliche' Parameter help genutzt werden.)
		/// </summary>
		public void AddAlias(string Param, string alias)
		{
			//wenn -s und --server möglich sind, und AddAlias("server", "s") aufgerufen wird,
			//dann wird ein möglicher Wert von -s in --server kopiert, wenn --server nicht bereits belegt ist.
			//im Rest vom Programm sollte man dann "server" verwenden
			if ((Parameters[Param]==null) && (Parameters[alias]!=null)) {
				Parameters.Add(Param, Parameters[alias]);
			}
		}
	}
}
