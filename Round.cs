using System;
using Mehroz;

namespace Mathe
{
	
	public class Round
	{
		//Wrappers by MarcK
		//return ist eigentlich nicht notwendig, da Arrays sowieso verändert werden
		public static Fraction[] Runden(Fraction[] b)
		{
			int m = Matrix.GetZeilenAnzahl(b);
			
			for (int no_m=1; no_m<=m; no_m++) {
				b[no_m] = Runden(b[no_m]);
			}
			
			return b;
		}
		
		public static Fraction[,] Runden(Fraction[,] A)
		{
			int m = Matrix.GetZeilenAnzahl(A); //Anzahl der Zeilen der Matrix
			int n = Matrix.GetSpaltenAnzahl(A); //Anzahl der Spalten der Matrix
			
			for (int no_m=1; no_m<=m; no_m++) {
				for (int no_n=1; no_n<=n; no_n++) {
					A[no_m, no_n] = Runden(A[no_m, no_n]);
				}
			}
			
			return A;
		}
	
		public static Fraction Runden(Fraction zahl)
		{
			return Runde(zahl.ToDouble());
		}

		public static double Runde(double zahl)
		{
			return Runde(zahl, 2);
		}
		
		
		public static double Runde(double zahl, int stellen)
		{
			if (zahl == 0)
			{
				return 0;
			}
			
			bool negativ = false;
			if (zahl < 0)
			{
				negativ = true;
				zahl = Math.Abs(zahl);
			}
			
			bool vorkomma = true;
			if (zahl < 1)
			{
				vorkomma = false;
			}

//			Console.WriteLine("");
//			Console.WriteLine("Zahl: " + zahl);
//			Console.WriteLine(new Fraction(zahl).ToString());
			
			int vorkommastellen = 0;
			if (vorkomma)
			{
				//Anzahl Stellen vor Komma berechnen
				vorkommastellen = Convert.ToInt32(Math.Truncate(Math.Log10(zahl)) + 1);
			}else{
				//Anzahl 0-Stellen nach Komma berechnen: 0.0042 -> -2
				vorkommastellen = Convert.ToInt32(Math.Truncate(Math.Log10(zahl)));
			}
			//Console.WriteLine("Vorkommastellen: " + vorkommastellen);
			
			//42.4711 -> 0.424711
			double zahlOhneVorkomma = zahl/Math.Pow(10, vorkommastellen);
//			Console.WriteLine("ZahlOhneVorkomma: " + zahlOhneVorkomma);
//			Console.WriteLine(new Fraction(zahlOhneVorkomma).ToString());
			
			//0.424711 -> 0.42
			zahlOhneVorkomma = Math.Round(zahlOhneVorkomma, stellen);
//			Console.WriteLine("Gerundet: " + vorkommastellen);
//			Console.WriteLine(new Fraction(zahlOhneVorkomma).ToString());
			
			//0.42 -> 42
			zahl = zahlOhneVorkomma*Math.Pow(10, vorkommastellen);
//			Console.WriteLine("Größer: " + zahl);
//			Console.WriteLine(new Fraction(zahl).ToString());
			
			if (negativ)
			{
				zahl = zahl*-1;
			}
			
			//Rundung, um interne Fehler (beim Vergrößern der Zahl) zu beseitigen.
			zahl = Math.Round(zahl, 10);
			
			return zahl;
		}
		
		//Test, um neue Rundungsfunktion gegen proprietäre zu testen.
		public static void RundungTest()
		{
			Random randObj = new Random();
			while(true)
			{
				int zaehler = randObj.Next(-1000, 1000);
				int nenner = randObj.Next(-1000, 1000);
				
				if (nenner == 0)
				{
					nenner = 1;
				}
				
				Fraction bruch = new Fraction(zaehler, nenner);
				
				double istRundung = Runde(bruch.ToDouble(), 2);
				double sollRundung = TRunde(bruch.ToDouble(), 2);
				
				Console.Write(bruch.ToString() + " ("+bruch.ToDouble() + "): " + istRundung + " vs " + sollRundung + " –– ");
				
				//if (istRundung == sollRundung)
				if (Math.Round(istRundung, 10) == Math.Round(sollRundung, 10))
				{
					Console.WriteLine("OK");
				}else{
					Console.WriteLine("FAIL:");
					Console.WriteLine("Ist:  " + new Fraction(istRundung).ToString());
					Console.WriteLine("Soll: " + new Fraction(sollRundung).ToString());
					Console.WriteLine("");
				}
			}
		}
		
		
		

		//Rest by Tobi
		public static double TRundenVormKomma(int zahl, int signifikante_stellen)
		{
			int stellen = Convert.ToInt32(Math.Log10(Convert.ToDouble(zahl)));
			double erg = 0.0;
			erg += zahl / (Math.Pow(10, stellen));
			erg = TRunde(erg, signifikante_stellen);
			erg *= Math.Pow(10, stellen);
			return erg;
		} 
		
		public static double TRunde(double zahl, int signifikante_stellen)
		{
			if (zahl == 0.0)
				return 0.0;
			
			bool negative = false;
			if (zahl < 0)
			{
				negative = true;
				zahl = Math.Abs(zahl);
			}
			
			int decimals = Convert.ToInt32(signifikante_stellen - (Math.Truncate(Math.Log10(zahl)) + 1));
			
			if (decimals < 0)
			{
				zahl = TRundenVormKomma(Convert.ToInt32(Math.Truncate(zahl)), signifikante_stellen);
				if (negative)
					return (zahl - 2 * zahl);
				else
					return zahl;
			}
			
			double erg = 0.0;
			if (zahl > 1)
				erg = Math.Round(zahl, decimals);
			else
				erg = Math.Round(zahl, decimals + 1);
			
			if (negative)
				return (erg - 2 * erg);
			else
				return erg;
		}		

	}
}
