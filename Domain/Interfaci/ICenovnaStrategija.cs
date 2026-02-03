using Domain.Modeli;
using Domain.Modeli.Enumeracije;

namespace Domain.Interfaci
{
    public interface ICenovnaStrategija
    {
        double IzracunajCenu(Vino vino);
        bool PrimenjivZa(KategorijaVina kategorija);
    }
}