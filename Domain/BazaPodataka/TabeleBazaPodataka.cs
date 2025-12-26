using Domain.Modeli;

namespace Domain.BazaPodataka
{
    public class TabeleBazaPodataka
    {
        public List<Korisnik> Korisnici { get; set; } = new List<Korisnik>();
        public List<Loza> Loze { get; set; } = new List<Loza>();
        public List<Vino> Vina { get; set; } = new List<Vino>();
        public List<Paleta> Palete { get; set; } = new List<Paleta>();
        public List<VinskiPodrum> VinskiPodrumi { get; set; } = new List<VinskiPodrum>();
        public List<Faktura> Fakture { get; set; } = new List<Faktura>();

        public TabeleBazaPodataka() { }
    }
}
