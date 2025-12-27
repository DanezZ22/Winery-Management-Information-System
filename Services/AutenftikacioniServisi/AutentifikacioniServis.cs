using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;

namespace Services.AutenftikacioniServisi
{
    public class AutentifikacioniServis : IAutentifikacijaServis
    {
        private readonly IKorisniciRepozitorijum korisniciRepozitorijum;
        private readonly ILoggerServis loggerServis;

        public AutentifikacioniServis(IKorisniciRepozitorijum korisniciRepozitorijum, ILoggerServis loggerServis)
        {
            this.korisniciRepozitorijum = korisniciRepozitorijum;
            this.loggerServis = loggerServis;
        }

        public (bool, Korisnik) Prijava(string korisnickoIme, string loznika)
        {
            try
            {
                Korisnik korisnik = korisniciRepozitorijum.PronadjiKorisnikaPoKorisnickomImenu(korisnickoIme);

                if(korisnik.KorisnickoIme == string.Empty)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Neuspešan pokušaj prijave: korisnik '{korisnickoIme}' ne postoji");
                    return (false, new Korisnik());
                }

                if(korisnik.Lozinka != loznika)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Neuspešan pokušaj prijave: pogrešna lozinka za korisnika '{korisnickoIme}'");
                }
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Uspešna prijava korisnika '{korisnickoIme}' ({korisnik.Uloga})");
                return (true, korisnik);
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greška pri prijavi: {ex.Message}");
                return (false, new Korisnik());
            }
        }
    }
}
