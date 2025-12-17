using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli.Enumeracije;

namespace Domain.Modeli
{
    public class Vino
    {
        public long Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public KategorijaVina Kategorija { get; set; }
        public double Zapremina { get; set; } // u litrima 
        public string SifraSerije { get; set; } = string.Empty; // VN-2025-ID_VINA
        public long IdLoze { get; set; }
        public DateTime DatumFlasiranja { get; set; }

        public Vino() { }

        public Vino(string naziv, KategorijaVina kategorija, double zapremina, long idLoze)
        {
            Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Naziv = naziv;
            Kategorija = kategorija;
            Zapremina = zapremina;
            SifraSerije = $"VN-{DateTime.Now.Year}-{Id}";
            IdLoze = idLoze;
            DatumFlasiranja = DateTime.Now;
        }


    }
}
