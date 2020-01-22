using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja2
{
    public static class AlgorytmHelper
    {
        public static void Harmonogram(List<List<int>> ln, int[] pi, int[] p, int[] mi, int[] S, int n, int m)
        {
            for (int i = 0; i < n; i++)
            {
                S[i] = 0;
            }

            int[] Z = new int[n + 1];
            for (int i = 0; i < n; i++)
            {
                Z[i] = 0;
            }

            for (int i = 0; i < n; i++)
            {
                int op = pi[i];
                int ms = mi[op];
                S[op] = Math.Max(S[op], Z[ms]);
                Z[ms] = S[op] + p[op];
                foreach (var node in ln[op])
                {
                    if (S[node] < Z[ms])
                    {
                        S[node] = Z[ms];
                    }
                }
            }
        }
    }
}
