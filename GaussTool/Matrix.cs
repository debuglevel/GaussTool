using System;
using Mehroz;

namespace Debuglevel.Mathe
{
    public class Matrix
    {
        public static int GetSpaltenAnzahl(Fraction[,] x)
        {
            return x.GetLength(1) - 1; //Anzahl der Spalten der Matrix
        }

        public static int GetZeilenAnzahl(Fraction[,] x)
        {
            return x.GetLength(0) - 1; //Anzahl der Spalten der Matrix
        }

        public static int GetZeilenAnzahl(Fraction[] x)
        {
            return x.GetLength(0) - 1; //Anzahl der Spalten der Matrix
        }

        public static void Einheitsmatrix(Fraction[,] e)
        {
            int m = GetZeilenAnzahl(e); //Anzahl der Zeilen der Matrix
            int n = GetSpaltenAnzahl(e); //Anzahl der Spalten der Matrix

            for (int no_m = 1; no_m <= m; no_m++)
            {
                for (int no_n = 1; no_n <= n; no_n++)
                {
                    e[no_m, no_n] = 0;
                    if (no_m == no_n) e[no_m, no_n] = 1;
                }
            }
        }

    }
}
