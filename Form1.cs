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
        List<List<int>> ln;

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
            try
            {
                var loadFileForm = new LoadFileForm(Procesy);
                loadFileForm.ShowDialog(this);
                Wyswietl();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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
            LiczWszystko();
        }
        public void LiczWszystko()
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

            AlgorytmHelper.Harmonogram(ln, permutacja.ToArray(), lczas.ToArray(), lmaszyna.ToArray(), S, n, 9);

            gP = lczas.ToArray();
            gS = S;
            gMi = lmaszyna.ToArray();
            gJ = lnrzadanie.ToArray();
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics Graph = e.Graphics;           
            Graph.Clear(Color.LightGray);
            Pen blackPen = new Pen(Color.Black);
            SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
            SolidBrush greenBrush = new SolidBrush(Color.Green);
            SolidBrush redBrush = new SolidBrush(Color.Red);
            pictureBox1.Width = 1050;
            float maxTime = 0;

            //Szukamy najwiekszego (czas rozpoczecia zadania + czas jego trwania)
            for (int i = 1; i < gS.Length; i++)
            {
                if (gS[i] + gP[i] > maxTime)
                {
                    maxTime = gS[i] + gP[i];
                }                    
            }

            float mnoznikSzerokosciProstokata = 1000 / maxTime;

            for (int i=0; i<gS.Length; i++)
            {
              
                float x = gS[i] * mnoznikSzerokosciProstokata;
                float y = gMi[i] * 50;
                float rectangleHeight = 30;
                float rectangleWidth = gP[i] * mnoznikSzerokosciProstokata;

                if (gJ[i] == 1)
                {
                    Graph.FillRectangle(yellowBrush, x, y, rectangleWidth, rectangleHeight);
                }

                if (gJ[i] == 2)
                {
                    Graph.FillRectangle(greenBrush, x, y, rectangleWidth, rectangleHeight);
                }

                if (gJ[i] == 3)
                {
                    Graph.FillRectangle(redBrush, x, y, rectangleWidth, rectangleHeight);
                }
            }

            // Rysowanie pionowych kresek
            for (int i = 0; i < 12; i++)
            {
                Graph.DrawLine(blackPen, i * 91, 0, i * 91, 1000);
            }

            // Wyznacznie skali osi X
            label8.Text  = ((int)(maxTime / 11)).ToString();
            label9.Text  = ((int)(2 * maxTime / 11)).ToString();
            label10.Text = ((int)(3 * maxTime / 11)).ToString();
            label11.Text = ((int)(4 * maxTime / 11)).ToString();
            label12.Text = ((int)(5 * maxTime / 11)).ToString();
            label13.Text = ((int)(6 * maxTime / 11)).ToString();
            label14.Text = ((int)(7 * maxTime / 11)).ToString();
            label15.Text = ((int)(8 * maxTime / 11)).ToString();
            label16.Text = ((int)(9 * maxTime / 11)).ToString();
            label17.Text = ((int)(10 *maxTime / 11)).ToString();
            label18.Text = maxTime.ToString();
        }
    }
}
