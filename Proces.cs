using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja2
{
    public class Proces
    {
        public List<Zadanie> Zadania { get; set; }
        public int Priorytet;
        public string NazwaPrzedmiotu;
        public Proces()
        {
            Zadania = new List<Zadanie>();
        }
    }
}
