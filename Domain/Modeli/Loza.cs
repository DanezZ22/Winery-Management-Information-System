using Domain.Modeli.Enumeracije;

namespace Domain.Modeli
{
    public class Loza
    {
        public long Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public double NivoSecera { get; set; } // 15.0 - 28.0 Brix
        public int GodinaSadnje { get; set; }
        public string RegionUzgoja { get; set; } = string.Empty;
        public FazaZrelosti FazaZrelosti { get; set; }

        public Loza() { }

        public Loza(string naziv, double nivoSecera, int godinaSadnje, string regionUzgoja)
        {
            Naziv = naziv;
            NivoSecera = nivoSecera;
            GodinaSadnje = godinaSadnje;
            RegionUzgoja = regionUzgoja;
            FazaZrelosti = FazaZrelosti.Posadjena;
        }
    }
}
