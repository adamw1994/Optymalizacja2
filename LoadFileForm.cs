using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Optymalizacja2
{
    public partial class LoadFileForm : Form
    {
        public List<Proces> Procesy { get; set; }
        public LoadFileForm(List<Proces> procesy)
        {
            this.Procesy = procesy;
            InitializeComponent();
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileReader = new FileReader();
            var fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var proces = fileReader.WczytajProces(fileDialog.FileName);
                proces.Priorytet = Int32.Parse(priorityTextBox.Text);
                Procesy.Add(proces);
            }
            this.Close();
        }

        private void priorityTextBox_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
    }
}
