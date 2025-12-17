using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modeli
{
    public class VinskiPodrum
    {
        public long Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public double TemperaturaSkladistenja { get; set; }
        public int MaxBrojPaleta { get; set; }
        public List<long> IdPaleta { get; set; } = new List<long>();

        public VinskiPodrum() { }

        public VinskiPodrum(string naziv, double temperaturaSkladistenja, int maxBrojPaleta)
        {
            Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Naziv = naziv;
            TemperaturaSkladistenja = temperaturaSkladistenja;
            MaxBrojPaleta = maxBrojPaleta;
        }
    }
}
