using Domain.Modeli.Enumeracije;

namespace Domain.Interfaci
{
    public interface ILoggerServis
    {
        public bool EvidentirajDogadjaj(TipEvidencije tip, string poruka);
    }
}
