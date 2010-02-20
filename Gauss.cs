using System;
using Mehroz;
using Mathe;

namespace GaussTool
{
	
	
	public class Gauss
	{
	
		public static void DoGauss(bool ROUND, bool GJ, bool INV, Fraction[,] A, Fraction[] b, Fraction[,] e)
		{
			int m = Matrix.GetZeilenAnzahl(A);
			int n = Matrix.GetSpaltenAnzahl(A);
			
			for (int k=1; k<=m; k++) {
				if (NotOnlyEmptyLinesRemaining(k, A)) {
					int jk = PivotSpalte(k, A);
					int ik = PivotZeile(k, PivotSpalte(k, A), A);
					if (ik != k) { SwapLines(k, ik, A, b, e); }

					//Gauss
					if (!GJ)
					{
						for (int i=k+1; i<=m; i++)
						{
							Fraction factor = (-1)*A[i,jk]/A[k,jk];
							if (ROUND) factor = Round.Runden(factor);
							AddFactorFromTo(factor, k, i, jk, ROUND, false, A, b, e);
						}
					}
					
					//Gauss-Jordan
					if (GJ)
					{	
						Fraction divisor = A[k, jk];
						if (ROUND) divisor = Round.Runden(divisor);
						for (int no_n=1; no_n<=n; no_n++) {
							A[k, no_n] = A[k, no_n]/divisor; 
							if (INV) e[k, no_n] = e[k, no_n]/divisor;
							if (ROUND)
							{
								A[k, no_n] = Round.Runden(A[k, no_n]);
								e[k, no_n] = Round.Runden(e[k, no_n]);
							}
						}
						b[k] = b[k]/divisor;
						if (ROUND) b[k] = Round.Runden(b[k]);

						for (int i=1; i<=m; i++)
						{
							if (i!=k) {
								Fraction factor = (-1)*A[i,jk];
								if (ROUND) factor = Round.Runden(factor);
								AddFactorFromTo(factor, k, i, jk, ROUND, INV, A, b, e);
							}
						}
					}
					
					Console.Write("LGS nach Umformung (Schritt ");
					CLI.WriteColored(k.ToString(), ConsoleColor.Yellow);
					Console.WriteLine("):");
					
					CLI.PrintLGS(INV);
					Console.WriteLine();
				}
			}
		}
		
		//TODO: hier sind nach meiner Erinnerung noch Fehler drin
		static void SwapLines(int k, int ik, Fraction[,] A, Fraction[] b, Fraction[,] e) {
			int n = Matrix.GetSpaltenAnzahl(A);
			
			Fraction[] Atemp = new Fraction[n+1];
			Fraction[] etemp = new Fraction[n+1];
			Fraction btemp = new Fraction();
			
			for (int no_n=1; no_n<=n; no_n++) {
				Atemp[no_n] = A[k,no_n];
				A[k, no_n] = A[ik,no_n];
				A[ik,no_n] = Atemp[no_n];
			}
			
			for (int no_n=1; no_n<=n; no_n++) {
				etemp[no_n] = e[k,no_n];
				e[k, no_n] = e[ik,no_n];
				e[ik,no_n] = etemp[no_n];
			}
			
			btemp = b[k];
			b[k] = b[ik];
			b[ik] = btemp;
		}
		
		static void AddFactorFromTo(Fraction factor, int from, int to, int aktuelleSpalte, bool ROUND, bool INV, Fraction[,] A, Fraction[] b, Fraction[,] e) {
			int n = Matrix.GetSpaltenAnzahl(A);
			for (int no_n=1; no_n<=n; no_n++) {
				//A[i,no_n] = A[i,no_n]+factor*A[k,no_n];
				//from = k
				//to = i
				Fraction temp = factor*A[from,no_n];
				Fraction tempe = factor*e[from,no_n];
				if (ROUND) temp = Round.Runden(temp);
				if (ROUND) tempe = Round.Runden(tempe);
				
				A[to,no_n] = A[to,no_n]+temp;
				if (INV) e[to,no_n] = e[to,no_n]+tempe;
				if (ROUND)
				{
					if (INV) e[to,no_n] = Round.Runden(e[to,no_n]);
					A[to,no_n] = Round.Runden(A[to,no_n]);
				}
				
				if (ROUND && (no_n == aktuelleSpalte)) A[to,no_n] = 0;  
			}
			
			
			Fraction temp2 = factor*b[from];
			if (ROUND) temp2 = Round.Runden(temp2);
			
			b[to] = b[to]+temp2;
			if (ROUND) b[to] = Round.Runden(b[to]);
		}
		
		static int PivotSpalte(int k, Fraction[,] A) {
			int n = Matrix.GetSpaltenAnzahl(A);
			int m = Matrix.GetZeilenAnzahl(A);
			
			for (int no_n=1; no_n <= n; no_n++) {
				for (int no_m=k; no_m <= m; no_m++) {
					if (A[no_m,no_n]!=0) {
						return no_n;
					}
				}
			}
			return -1;
		}

		static int PivotZeile(int k, int Spalte, Fraction[,] A) {
			int m = Matrix.GetZeilenAnzahl(A);
			for (int no_m=k; no_m <= m; no_m++) {
				if (A[no_m,Spalte]!=0) {
					return no_m;
				}
			}
			return -1;
		}
			
		//In den Zeilen unterhalb(oder gleich) von k ist nicht jede Spalte == 0
		static bool NotOnlyEmptyLinesRemaining(int k, Fraction[,] A) {
			int m = Matrix.GetZeilenAnzahl(A);
			int n = Matrix.GetSpaltenAnzahl(A);
			
			for (int no_m=k; no_m <= m; no_m++) {
				for (int no_n=1; no_n <= n; no_n++) {
					if(A[no_m, no_n]!=0) {
						return true;
					}
				}
			}
			
			return false;
		} 


		
	}
}
