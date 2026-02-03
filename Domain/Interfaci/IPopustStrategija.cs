using Domain.Modeli.Enumeracije;

namespace Domain.Interfaci
{
    public interface IPopustStrategija
    {
        double PrimeniPopust(double cena);
        bool PrimenjivZa(TipProdaje tipProdaje);
    }
}