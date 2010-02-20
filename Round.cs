//by Tobias Kopp

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

		//Rest by Tobi
		public static double RundenVormKomma(int zahl, int signifikante_stellen)
		{
			int stellen = Convert.ToInt32(Math.Log10(Convert.ToDouble(zahl)));
			double erg = 0.0;
			erg += zahl / (Math.Pow(10, stellen));
			erg = Runde(erg, signifikante_stellen);
			erg *= Math.Pow(10, stellen);
			return erg;
		} 
		
		public static double Runde(double zahl, int signifikante_stellen)
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
				zahl = RundenVormKomma(Convert.ToInt32(Math.Truncate(zahl)), signifikante_stellen);
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
