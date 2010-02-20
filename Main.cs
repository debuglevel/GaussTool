/*
 * License: GPLv3 (I actually don't care about v2, v3 or even another OSS-License. Just edit the code, share it, tell me about it, and keep my website in the header.) 
 * http://www.debuglevel.de
 * Author: Marc Kohaupt, Tobias Kopp (rounding-related functions)
*/

using System;
using System.IO;
using CommandLine.Utility;
using Mehroz;
using Mathe;

namespace GaussTool
{
	class CLI
	{

		public static int m;
		public static int n;
		public static Fraction[,] A;
		public static Fraction[] b;
		public static Fraction[,] e;
		public static bool debug=false;
		
		public static void Main(string[] args)
		{
			
			Arguments clArgs = new Arguments(args);
			
			bool fromFile = false;
			bool doRound = false;
			bool doFrac = false;
			bool gaussjordan = false;
			bool inverse = false;
			

			//Rundung-Tests
			clArgs.AddAlias("roundtest", "rt");
			if (clArgs["roundtest"] != null)
			{
				Round.RundungTest();
				return;
			}
			
			
			//change forecolor to black 
			clArgs.AddAlias("schwarz", "s");
			if (clArgs["schwarz"] != null)
			{
				Console.ForegroundColor = ConsoleColor.Black;
			}

			
			clArgs.AddAlias("bruch", "b");
			if (clArgs["bruch"] != null)
			{
				Console.WriteLine("· Bruchmodus aktiviert");
				doFrac = true;
			}

			
			clArgs.AddAlias("rundung", "r");
			int runden_stellen = 0;
			if (clArgs["rundung"] != null && Int32.TryParse(clArgs["rundung"], out runden_stellen))
			{
				Console.WriteLine("· Rundungsmodus ({0} gültige Stellen) aktiviert", runden_stellen);
				doRound = true;
			}
			else if(clArgs["rundung"] != null)
			{
				WriteLineColored("Bitte Anzahl gültiger Stellen bei -r angeben.", ConsoleColor.Red);
				return;
			}
			
			/* Mono hat in Version ~1.9.1 ein Bug */
			if (System.Environment.OSVersion.Platform == PlatformID.Unix && clArgs["rundung"] != null)
			{
				WriteLineColored("Achtung! Lieber Unix-User, leider hat Mono (zumindest 1.9.1) ein Problem mit der Rundung im -r Modus :-(", ConsoleColor.Red);
			}

			
			clArgs.AddAlias("gaussjordan", "gj");
			clArgs.AddAlias("gauss", "g");
			if (clArgs["gaussjordan"] != null && clArgs["gauss"] == null)
			{
				Console.WriteLine("· Gauß-Jordan aktiviert");
				gaussjordan = true;
			}
			else if (clArgs["gaussjordan"] != null && clArgs["gauss"] != null)
			{
				Console.WriteLine("Entweder -g oder -gj auswählen");
				return;
			}
			else
			{
				Console.WriteLine("· Gauß aktiviert");
			}
			
			
			clArgs.AddAlias("inverse", "i");
			if (clArgs["inverse"] != null)
			{
				Console.WriteLine("· Inverse aktiviert (Gauß-Jordan aktiviert)");
				gaussjordan = true;				
				inverse = true;
			}

			
			clArgs.AddAlias("file", "f");
			if (clArgs["file"] != null && clArgs["file"] != "")
			{
				if (File.Exists(clArgs["file"]) == false)
				{
					Console.WriteLine("Datei "+clArgs["file"]+" existiert nicht.");
					return;
				}
				
				Console.WriteLine("· File-Input aktiviert: "+clArgs["file"]);
					
				fromFile = true;
			}
			
			clArgs.AddAlias("credits", "c");
			if (clArgs["credits"] != null)
			{
				credits();
				return;
			}
		
			clArgs.AddAlias("help", "h");
			if ((clArgs["help"] != null) || (doRound==false && doFrac==false))
			{
				if (doRound==false && doFrac==false)
				{
					help(true);
				}else{
					help(false);
				}
				return;
			}
			
			clArgs.AddAlias("debug", "d");
			if (clArgs["debug"] != null)
			{
				debug = true;
			}
			
			
			Console.Write("Anzahl der Unbekannten: ");
			n = Convert.ToInt32(Console.ReadLine());
			
			Console.Write("Anzahl der Gleichungen: ");
			m = Convert.ToInt32(Console.ReadLine());

			
			A = new Fraction[m+1,n+1];
			b = new Fraction[m+1];

			
			if (fromFile==true)
			{
				ReadFromFile(clArgs["file"]);
			}else{
				ReadFromConsole();
			}

			
			//Einheitsmatrix deklarieren und füllen
			e = new Fraction[m+1,n+1];
			Matrix.Einheitsmatrix(e);

			
			Fraction[,] copyA = new Fraction[m+1,n+1];
			Fraction[] copyb = new Fraction[m+1];
			copyA = (Fraction[,])A.Clone();
			copyb = (Fraction[])b.Clone();

			
			Console.WriteLine("\nDas LGS wurde wie folgt eingelesen:");
			PrintLGS(inverse);

			
			if (doFrac)
			{
				WriteLineColored("\nGauss mit Brüchen", ConsoleColor.Yellow);
				WriteLineColored("=================\n", ConsoleColor.Yellow);
				Gauss.DoGauss(false, gaussjordan, inverse, A, b, e);
			}
			
			if (doRound)
			{
				A = (Fraction[,])copyA.Clone();
				b = (Fraction[])copyb.Clone();
				Round.Runden(A);
				Round.Runden(b);
				
				WriteLineColored("\nGauss mit Rundungen", ConsoleColor.Yellow);
				WriteLineColored("===================\n", ConsoleColor.Yellow);
				
				Console.WriteLine("Das LGS wurde wie folgt gerundet:");
				PrintLGS(inverse);
				Console.WriteLine();
				
				Gauss.DoGauss(true, gaussjordan, inverse, A, b, e);
			}
			
		}
		
		static void help(bool missingArg)
		{
			Console.WriteLine(
@"GaussTool, Version: 2009-05-10_15-02, (Tobi K. & Marc K.)

-h, --help              Diese Hilfe ;-)
-b, --bruch             Bruchmodus aktivieren
-r N, --rundung N       Rundungsmodus (auf N gültige Stellen) aktivieren
-gj, --gaussjordan      Gauß-Jordan aktivieren (overwrites -g)
-g, --gauss             Gauß aktivieren (default, conflicts with -gj)
-i, --inverse           Inverse berechnen (implies -gj)
-f DATEI, --file DATEI  Zahlen aus DATEI lesen (durch ENTER getrennt,
                          Reihenfolge wie bei interaktiver Eingabe)
-s, --schwarz           Schwarze Vordergrundschrift verwenden (statt hellgrau)
-c, --credits           :^)
-d, --debug             Debugausgaben aktivieren");
				
			if (missingArg)
			{
				WriteLineColored("\nBitte gebe die Option -b oder -r an!", ConsoleColor.Red);
			}
		}
		
		
		
		static void ReadFromConsole()
		{
			for (int no_m=1; no_m<=m; no_m++) {
				for (int no_n=1; no_n<=n; no_n++) {
					Console.Write("A[{0},{1}] := ", no_m, no_n);
					A[no_m, no_n] = Console.ReadLine();
				}
			}

			for (int no_m=1; no_m<=m; no_m++) {
				Console.Write("b[{0}] := ", no_m);
				b[no_m] = Console.ReadLine();
			}
		}
		
		static void ReadFromFile(string file)
		{
			StreamReader sr = new StreamReader(file);
			
			for (int no_m=1; no_m<=m; no_m++) {
				for (int no_n=1; no_n<=n; no_n++) {
					A[no_m, no_n] = sr.ReadLine();
					Debug("A["+no_m+","+no_n+"] = "+A[no_m, no_n].ToString()+"\n");
				}
			}

			for (int no_m=1; no_m<=m; no_m++) {
				b[no_m] = sr.ReadLine();
			}
		}
		
		static void Debug(string s)
		{
			if (debug)
			{
				Console.Write(s);
			}
		}
		
		public static void PrintLGS(bool INV) {
			Console.WriteLine("-------------------");
			
			int[] width_A = new int[n+1];
			int[] width_e = new int[n+1];
			int width_b = 1;
			
			//Spaltenbreite berechnen
			for (int no_n=1; no_n<=n; no_n++) {
				for (int no_m=1; no_m<=m; no_m++) {
					if (width_A[no_n] < A[no_m, no_n].ToString().Length)
					{
						width_A[no_n] = A[no_m, no_n].ToString().Length;
						Debug("width_A["+no_n+"]="+width_A[no_n]+"\n");
					}
				}
			}
			
			for (int no_n=1; no_n<=n; no_n++) {
				for (int no_m=1; no_m<=m; no_m++) {
					if (width_e[no_n] < e[no_m, no_n].ToString().Length)
					{
						width_e[no_n] = e[no_m, no_n].ToString().Length;
						Debug("width_e["+no_n+"]="+width_e[no_n]+"\n");
					}
				}
			}
			
			for (int no_m=1; no_m<=m; no_m++) {
				if (width_b < b[no_m].ToString().Length)
				{
					width_b = b[no_m].ToString().Length;
					Debug("width_b="+width_b+"\n");
				}
			}
			
			
			for (int no_m=1; no_m<=m; no_m++) {
				for (int no_n=1; no_n<=n; no_n++) {
					Console.Write("{0}", A[no_m, no_n].ToString().PadLeft(width_A[no_n]+1));
				}

				Console.Write(" |");
				
				if (INV)
				{
					for (int no_n=1; no_n<=n; no_n++) {
						Console.Write("{0}", e[no_m, no_n].ToString().PadLeft(width_e[no_n]+1));
					}
				}
				else
				{
					Console.Write("{0}", b[no_m].ToString().PadLeft(width_b+1));
				}
				
				Console.Write("\n");
			}
			Console.WriteLine("-------------------");
			PrintMapleCompliant(INV);
		}
		
		public static void PrintMapleCompliant(bool INV) {
			string s="";
			s="[";
			for (int no_m=1; no_m<=m; no_m++) {
				s+="[";

				for (int no_n=1; no_n<=n; no_n++) {
					s+=A[no_m, no_n].ToString()+",";
				}
				s=s.Substring(0, s.Length-1); //Komma abschneiden
				
				s+="],";
			}
			s=s.Substring(0, s.Length-1); //Komma abschneiden
			s+="]";

			ResizeWindow(s);
			WriteLineColored(s, ConsoleColor.Green);
			
			s="";
			if (INV)
			{
				s="[";
				for (int no_m=1; no_m<=m; no_m++) {
					s+="[";
					
					for (int no_n=1; no_n<=n; no_n++) {
						s+=e[no_m, no_n].ToString()+",";
					}
					s=s.Substring(0, s.Length-1); //Komma abschneiden
					
					s+="],";
				}
				s=s.Substring(0, s.Length-1); //Komma abschneiden
				s+="]";
			}
			else
			{
				s="[";
				for (int no_n=1; no_n<=n; no_n++) {
					s+=b[no_n].ToString()+",";
				}
				s=s.Substring(0, s.Length-1); //Komma abschneiden
				s+="]";
			}
			
			ResizeWindow(s);
			WriteLineColored(s, ConsoleColor.DarkGreen);
		}
		
		public static void WriteColored(string s, ConsoleColor c)
		{
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = c;
			Console.Write(s);
			Console.ForegroundColor = color;
		}
		
		public static void WriteLineColored(string s, ConsoleColor c)
		{
			WriteColored(s+"\n", c);
		}
		
		static void ResizeWindow(string s)
		{
			if (Console.WindowWidth < (s.Length+1))
			{
				try
				{
					Console.WindowWidth = s.Length+1;
				}
				catch(Exception ex)
				{
					Debug("Wollte Breite des Terminals vergrößern - ging nicht!");
				}
				
			}
		}
		
		static void credits()
		{
			Console.WriteLine(@"Persönlicher Dank geht an:
· Tobi, für Runden() und schnelle Fixes
· Heinrich, für den Report eines fatalen Fehlers,
· miguel, für Mono
· Prof. Dr. Morgenstern, für den Ansporn
· Herr Gauß, für den Algorithmus
· Project Pitchfork & Blutengel, für meine Hintergrundmusik");

			string lwss=@"
      WWWWWWWW
      WWWWWWWW
    WW      WW
    WW      WW

    WW    WW
    WW    WW

";
			ConsoleColor[] colors = {ConsoleColor.DarkRed, ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.DarkMagenta};
			
			Random rand = new Random();
			for (int i=0; i<lwss.Length; ++i)
			{
				WriteColored(lwss[i].ToString(), colors[rand.Next(colors.Length)]);
			}
		}

		
	}
	
}