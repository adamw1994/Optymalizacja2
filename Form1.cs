using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Optymalizacja2
{
    public partial class Form1 : Form
    {
        private FileReader fileReader;
        private List<Proces> Procesy = new List<Proces>();

        public DataSet DS;
        public DataTable DT;
        List<List<int>> ln = new List<List<int>>();
        int[] gS = null;
        int[] gP = null;
        int[] gMi = null;
        int[] gJ = null;

        // Konstruktor - wywoluje sie przy starcie programu
        public Form1()
        {
            InitializeComponent();
            dataGridView1.ReadOnly = true;
            fileReader = new FileReader();
        }


        // Funkcja ktora odpala sie po kliknieciu - wczytaj plik
        private void WczytajPlik_Click(object sender, EventArgs e)
        {
            var loadFileForm = new LoadFileForm(Procesy);
            loadFileForm.ShowDialog(this);
            Wyswietl();
        }

        // Funkcja do wyswietlania danych w DataGridzie
        private void Wyswietl()
        {
            DS = new DataSet();
            DT = DS.Tables.Add("dane");

            DT.Columns.Add("nr zadania");
            DT.Columns.Add("nr");
            DT.Columns.Add("Zadanie");
            DT.Columns.Add("Czas");
            DT.Columns.Add("Maszyna");
            DT.Columns.Add("Wymaga");
            DT.Columns.Add("Następnik");

            foreach (var proces in Procesy)
            {
                foreach (var zadanie in proces.Zadania)
                {
                    DataRow DR = DT.NewRow();
                    DR[0] = proces.Priorytet;
                    DR[1] = zadanie.Numer;
                    DR[2] = zadanie.Nazwa;
                    DR[3] = zadanie.Czas;
                    DR[4] = zadanie.IdMaszyny;
                    DR[5] = zadanie.Poprzednik;
                    DR[6] = zadanie.Nastepnik;
                    DT.Rows.Add(DR);
                }
            }
            DT.Select();
            dataGridView1.DataSource = DT;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AlgorytmJakis();

        }
        public void AlgorytmJakis()
        {
            int oldZad = 0;
            int n = 0;

            int Offset = 0;
            ln = new List<List<int>>();
            ln.Add(new List<int>());

            List<int> lczas = new List<int>();
            lczas.Add(0);

            List<int> lmaszyna = new List<int>();
            lmaszyna.Add(0);

            List<int> lnrzadanie = new List<int>();
            lnrzadanie.Add(0);


            foreach (var row in DT.Select())
            {
                n = n + 1;
                string nrZadania = row["nr zadania"].ToString();
                string nr = row["nr"].ToString();
                string czas = row["Czas"].ToString();
                string maszyna = row["Maszyna"].ToString();
                string wymaga = row["Wymaga"].ToString();
                string nastepnik = row["Następnik"].ToString();

                int inrZadania = int.Parse(nrZadania);
                int inr = int.Parse(nr);
                int iCzas = int.Parse(czas);
                int iMaszyna = int.Parse(maszyna);
                // int iWymaga = int.Parse(wymaga);

                lczas.Add(iCzas);
                lmaszyna.Add(iMaszyna);
                lnrzadanie.Add(inrZadania);


                string[] sp = nastepnik.Split(',');

                if (oldZad != inrZadania)
                {
                    oldZad = inrZadania;
                    Offset = n - 1;
                }

                List<int> nst = new List<int>();



                foreach (string s in sp)
                {
                    if (s != "")
                    {
                        var node = int.Parse(s) + Offset;
                        nst.Add(node);
                    }

                }
                ln.Add(nst);
            }
            List<int> permutacja = new List<int>();
            for (int i = 0; i < n; i++)
            {
                permutacja.Add(i);
            }
            int[] S = new int[n + 1];
            Harmonogram(ln, permutacja.ToArray(), lczas.ToArray(), lmaszyna.ToArray(), S, n, 9);
            gP = lczas.ToArray();
            gS = S;
            gMi = lmaszyna.ToArray();
            gJ = lnrzadanie.ToArray();
            pictureBox1.Refresh();
        }

        public void Harmonogram(List<List<int>> ln, int [] pi, int [] p, int [] mi, int [] S, int n, int m)
        {
            for (int i=0; i<n; i++)
            {
                S[i] = 0;
            }

            int[] Z = new int[n + 1];
            for (int i = 0; i < n; i++)
            {
                Z[i] = 0;
            }

            for (int i=0; i<n; i++)
            {
                int op = pi[i];
                int ms = mi[op];
                S[op] = Math.Max(S[op], Z[ms]);
                Z[ms] = S[op] + p[op];
                foreach(var node in ln[op])
                {
                    if (S[node] < Z[ms])
                    {
                        S[node] = Z[ms];
                    }
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics G = e.Graphics;
           
            G.Clear(Color.LightGray);
            Pen px = new Pen(Color.Black);
            SolidBrush br = new SolidBrush(Color.Yellow);
            SolidBrush br1 = new SolidBrush(Color.Green);
            SolidBrush br2 = new SolidBrush(Color.Red);
            //     G.FillRectangle(br, 100, 100, 200, 200);

            if (gS == null)
            {
                return;
            }

            int graphWidth = 0;

            for(int i=1;i<gS.Length;i++)
            {
               

                int x = gS[i] * 10;
                int w = gP[i] * 30;
                int y = gMi[i] * 50;
                int h = 30;
                graphWidth += gP[i];
                if (gJ[i] == 1)
                {
                    G.FillRectangle(br, x, y, w, h);
                }

                if (gJ[i] == 2)
                {
                    G.FillRectangle(br1, x, y, w, h);
                }

                if (gJ[i] == 3)
                {
                    G.FillRectangle(br2, x, y, w, h);
                }

            }
            for (int i = 1; i < 12; i++)
            {
                G.DrawLine(px, i * pictureBox1.Width/11, 0, i * pictureBox1.Width/11, 1000);
            }
            pictureBox1.Width = gS[gS.Length-1] * 10 + gP[gP.Length-1]*30 + 50;
            label8.Text = (graphWidth / 10).ToString();
            label9.Text = (2 * graphWidth / 10).ToString();
            label10.Text = (3 * graphWidth / 10).ToString();

        }


    }
}
