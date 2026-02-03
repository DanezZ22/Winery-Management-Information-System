using Domain.Interfaci;
using Domain.Modeli.Enumeracije;

namespace Services.PopustStrategije
{
    public class BezPopustaStrategija : IPopustStrategija
    {
        public double PrimeniPopust(double cena)
        {
            return cena;
        }

        public bool PrimenjivZa(TipProdaje tipProdaje)
        {
            return tipProdaje == TipProdaje.RestoranskaProadaja;
        }
    }
}