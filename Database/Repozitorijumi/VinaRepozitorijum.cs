using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Modeli.Enumeracije;

namespace Database.Repozitorijumi
{
    public class VinaRepozitorijum : IVinaRepozitorijum
    {
        private readonly IBazaPodataka bazaPodataka;

        public VinaRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }

        public Vino DodajVino(Vino vino)
        {
            try
            {
                vino.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + bazaPodataka.Tabele.Vina.Count;
                vino.SifraSerije = $"VN-{DateTime.Now.Year}-{vino.Id}";
                bazaPodataka.Tabele.Vina.Add(vino);
                bazaPodataka.SacuvajPromene();
                return vino;
            }
            catch
            {
                return new Vino();
            }
        }

        public Vino PronadjiVinoPoId(long id)
        {
            try
            {
                return bazaPodataka.Tabele.Vina.FirstOrDefault(v => v.Id == id) ?? new Vino();
            }
            catch
            {
                return new Vino();
            }
        }

        public IEnumerable<Vino> SvaVina()
        {
            try
            {
                return bazaPodataka.Tabele.Vina;
            }
            catch
            {
                return new List<Vino>();
            }
        }

        public IEnumerable<Vino> PronadjiVinaPoKategoriji(KategorijaVina kategorija)
        {
            try
            {
                return bazaPodataka.Tabele.Vina.Where(v => v.Kategorija == kategorija);
            }
            catch
            {
                return new List<Vino>();
            }
        }

        public bool AzurirajVino(Vino vino)
        {
            try
            {
                var postojeceVino = bazaPodataka.Tabele.Vina.FirstOrDefault(v => v.Id == vino.Id);
                if (postojeceVino != null)
                {
                    int index = bazaPodataka.Tabele.Vina.IndexOf(postojeceVino);
                    bazaPodataka.Tabele.Vina[index] = vino;
                    bazaPodataka.SacuvajPromene();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ObrisiVino(long id)
        {
            try
            {
                var vino = bazaPodataka.Tabele.Vina.FirstOrDefault(v => v.Id == id);
                if (vino != null)
                {
                    bazaPodataka.Tabele.Vina.Remove(vino);
                    bazaPodataka.SacuvajPromene();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}