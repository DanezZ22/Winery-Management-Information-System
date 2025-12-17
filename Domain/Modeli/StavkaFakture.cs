using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modeli
{
    public class StavkaFakture
    {
        public long IdVina { get; set; }
        public int Kolicina { get; set; }
        public double CenaPoJedinici { get; set; }
        public double Ukupno => Kolicina * CenaPoJedinici;

        public StavkaFakture() { }

        public StavkaFakture(long idVina, int kolicina, double cenaPoJedinici)
        {
            IdVina = idVina;
            Kolicina = kolicina;
            CenaPoJedinici = cenaPoJedinici;
        }
    }
}
