using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repozitorijumi
{
    public class KorisniciRepozitorijum : IKorisniciRepozitorijum
    {
        private readonly IBazaPodataka bazaPodataka;

        public KorisniciRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }

        public Korisnik DodajKorisnika(Korisnik korisnik)
        {
            try
            {
                korisnik.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + bazaPodataka.Tabele.Korisnici.Count;



                bazaPodataka.Tabele.Korisnici.Add(korisnik);
                bazaPodataka.SacuvajPromene();


                return korisnik;
            }
            catch
            {
                return new Korisnik();
            }
        }
        public Korisnik PronadjiKorisnikaPoKorisnickomImenu(string korisnickoIme)
        {
            try
            {
                return bazaPodataka.Tabele.Korisnici.FirstOrDefault(k => k.KorisnickoIme == korisnickoIme) ?? new Korisnik();
            }
            catch
            {
                return new Korisnik();
            }
        }
        public IEnumerable<Korisnik> SviKorisnici()
        {
            try
            {
                return bazaPodataka.Tabele.Korisnici;
            }
            catch
            {
                return new List<Korisnik>();
            }
        }
    }
}
