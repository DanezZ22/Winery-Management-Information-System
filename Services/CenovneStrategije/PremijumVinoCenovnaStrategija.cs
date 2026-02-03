using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;

namespace Services.CenovneStrategije
{
    public class PremijumVinoCenovnaStrategija : ICenovnaStrategija
    {
        private const double BAZNA_CENA_PO_LITRU = 35.0;

        public double IzracunajCenu(Vino vino)
        {
            return vino.Zapremina * BAZNA_CENA_PO_LITRU;
        }

        public bool PrimenjivZa(KategorijaVina kategorija)
        {
            return kategorija == KategorijaVina.PremijumVino;
        }
    }
}