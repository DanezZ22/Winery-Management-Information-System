using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Modeli.Enumeracije;

namespace Domain.Modeli
{
    public class Faktura
    {
        public long Id { get; set; }
        public TipProdaje TipProdaje { get; set; }
        public NacinPlacanja NacinPlacanja { get; set; }
        public List<StavkaFakture> Stavke { get; set; } = new List<StavkaFakture>();
        public DateTime DatumKreiranja { get; set; }
        public double UkupanIznos => Stavke.Sum(s => s.Ukupno);

        public Faktura() { }

        public Faktura(TipProdaje tipProdaje, NacinPlacanja nacinPlacanja)
        {
            Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            TipProdaje = tipProdaje;
            NacinPlacanja = nacinPlacanja;
            DatumKreiranja = DateTime.Now;
        }
    }
}
