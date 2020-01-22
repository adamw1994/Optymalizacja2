using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja2
{
    public class Zadanie
    {
        public string Numer { get; set; }
        public string Nazwa { get; set; }
        public string IdMaszyny { get; set; }
        public string Poprzednik { get; set; }
        public string Nastepnik { get; set; }
        public string Czas { get; set; }
    }
}
