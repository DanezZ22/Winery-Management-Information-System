using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class LozeRepozitorijum : ILozeRepozitorijum
    {
        private readonly IBazaPodataka bazaPodataka;

        public LozeRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }
        public Loza DodajLozu(Loza loza)
        {
            try {
                loza.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + bazaPodataka.Tabele.Loze.Count;



                bazaPodataka.Tabele.Loze.Add(loza);
                bazaPodataka.SacuvajPromene();


                return loza;
            }
            catch 
            {
                return new Loza();
            }

        }
        public Loza PronadjiLozuPoId(long id)
        {
            try
            {
                return bazaPodataka.Tabele.Loze.FirstOrDefault(l => l.Id == id) ?? new Loza();
            }
            catch
            {
                return new Loza();
            }
        }
        public IEnumerable<Loza> SveLoze()
        {
            try
            {
                return bazaPodataka.Tabele.Loze;
            }
            catch 
            { 
                return new List<Loza>(); 
            }

        }
        public IEnumerable<Loza> PronadjiLozePoNazivu(string naziv)
        {
            try
            {
                return bazaPodataka.Tabele.Loze.Where(l => l.Naziv== naziv);
            }
            catch
            {
                return new List<Loza>();
            }
        }
        public IEnumerable<Loza> PronadjiLozePoFaziZrelosti(FazaZrelosti faza)
        {
            try
            {
                return bazaPodataka.Tabele.Loze.Where(l => l.FazaZrelosti == faza);
            }
            catch
            {
                return new List<Loza>();
            }
        }
        public bool AzurirajLozu(Loza loza)
        {
            try 
            {
                var postojecaLoza = bazaPodataka.Tabele.Loze.FirstOrDefault(l => l.Id == loza.Id);
                if (postojecaLoza != null)
                {
                    int index = bazaPodataka.Tabele.Loze.IndexOf(postojecaLoza);



                    bazaPodataka.Tabele.Loze[index] = loza;
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
        public bool ObrisiLozu(long id)
        {
            try
            {
                var postojecaLoza = bazaPodataka.Tabele.Loze.FirstOrDefault(l => l.Id == id);
                if (postojecaLoza != null)
                {
                    bazaPodataka.Tabele.Loze.Remove(postojecaLoza);
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
