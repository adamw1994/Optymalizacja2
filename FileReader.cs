using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optymalizacja2
{
    public class FileReader
    {
        public DataSet DS;
        public List<Proces> Procesy;
        public FileReader()
        {
            DS = new DataSet();
        }

        public List<string> LoadToListANSI(string name)
        {
            Encoding encoding = Encoding.GetEncoding(1250);
            try
            {
                var readedFile = File.ReadAllLines(name, encoding);
                var L = new List<string>();

                foreach (var s in readedFile) L.Add(s);
                return L;
            }
            catch
            {
                return null;
            }
        }
        
        public Proces WczytajProces(string path)
        {
            var proces = new Proces();
            proces.NazwaPrzedmiotu = Path.GetFileName(path);
            var ansiList = LoadToListANSI(path);
            if (ansiList == null) return null;

            try
            {
                string[] fields = ansiList[0].Split(';');
                ansiList.RemoveAt(0);

                foreach (var row in ansiList)
                {
                    if (row.Replace(';', ' ').Trim() == "") continue;
                    string[] sp = row.Split(';');
                    var zadanie = new Zadanie();

                    zadanie.Numer = sp[0];
                    zadanie.Nazwa = sp[1];
                    zadanie.Czas = sp[2];
                    zadanie.IdMaszyny = sp[3];
                    zadanie.Poprzednik = sp[4];
                    zadanie.Nastepnik = sp[5];
                    proces.Zadania.Add(zadanie);
                }

                return proces;
            }
            catch (Exception ex) 
            { 
                return null; 
            }
        }
    }
}
