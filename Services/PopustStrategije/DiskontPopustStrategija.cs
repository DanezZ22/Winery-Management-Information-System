using Domain.Interfaci;
using Domain.Modeli.Enumeracije;

namespace Services.PopustStrategije
{
    public class DiskontPopustStrategija : IPopustStrategija
    {
        private const double POPUST_PROCENAT = 0.15;

        public double PrimeniPopust(double cena)
        {
            return cena * (1 - POPUST_PROCENAT);
        }

        public bool PrimenjivZa(TipProdaje tipProdaje)
        {
            return tipProdaje == TipProdaje.DiskontPica;
        }
    }
}