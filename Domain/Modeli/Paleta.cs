using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Domain.Enumeracije;

namespace Domain.Modeli
{
    public class Paleta
    {
        public long Id { get; set; }
        public string Sifra { get; set; } = string.Empty;
        public string AdresaOdredista { get; set; } = string.Empty;
        public long IdVinskogPodruma { get; set; }
        public List<long> IdVina { get; set; } = new List<long>();
        public StatusPalete Status { get; set; }

        public Paleta() { }

        public Paleta(string adresaOdredista, long idVinskogPodruma)
        {
            Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Sifra = $"PAL-{DateTime.Now.Year}-{Id}";
            AdresaOdredista = adresaOdredista;
            IdVinskogPodruma = idVinskogPodruma;
            Status = StatusPalete.Upakovana;
        }

    }
}
