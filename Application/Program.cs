using Database.Repozitorijumi;
using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Presentation.Authentifikacija;
using Presentation.Meni;
using Services.AutenftikacioniServisi;
using Services.LoggerServisi;
using Services.PakovanjeServisi;
using Services.ProizvodnjaVinaServisi;
using Services.ProdajaServisi;
using Services.SkladistenjeServisi;
using Services.VinogradarstvoServisi;
using Domain.Interfaci;
using Domain.Modeli.Enumeracije;

namespace Loger_Bloger
{
    public class Program
    {
        public static void Main()
        {
            string putanjaDoBaze = Path.Combine(Directory.GetCurrentDirectory(), "vinarija.xml");
            IBazaPodataka bazaPodataka = new XmlBazaPodataka(putanjaDoBaze);

            string putanjaDoLoga = Path.Combine(Directory.GetCurrentDirectory(), "vinarija.log");
            ILoggerServis loggerServis = new LoggerServis(putanjaDoLoga);

            IKorisniciRepozitorijum korisniciRepozitorijum = new KorisniciRepozitorijum(bazaPodataka);
            ILozeRepozitorijum lozeRepozitorijum = new LozeRepozitorijum(bazaPodataka);
            IVinaRepozitorijum vinaRepozitorijum = new VinaRepozitorijum(bazaPodataka);
            IPaleteRepozitorijum paleteRepozitorijum = new PaleteRepozitorijum(bazaPodataka);
            IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum = new VinskiPodrumiRepozitorijum(bazaPodataka);
            IFaktureRepozitorijum faktureRepozitorijum = new FaktureRepozitorijum(bazaPodataka);

            IAutentifikacijaServis autentifikacijaServis = new AutentifikacioniServis(korisniciRepozitorijum, loggerServis);
            IVinogradarstvoServis vinogradarstvoServis = new VinogradarstvoServis(lozeRepozitorijum, loggerServis);
            IProizvodnjaVinaServis proizvodnjaVinaServis = new ProizvodnjaVinaServis(vinaRepozitorijum, lozeRepozitorijum, vinogradarstvoServis, loggerServis);
            IPakovanjeServis pakovanjeServis = new PakovanjeServis(vinaRepozitorijum, paleteRepozitorijum, loggerServis, vinskiPodrumiRepozitorijum);
            IVinskiPodrumServis vinskiPodrumServis = new VinskiPodrumServis(vinskiPodrumiRepozitorijum, loggerServis);

            ISkladistenjeServis vinskiPodrumSkladistenjeServis = new VinskiPodrumSkladistenjeServis(paleteRepozitorijum, loggerServis);
            ISkladistenjeServis lokalniKelarSkladistenjeServis = new LokalniKelarSkladistenjeServis(loggerServis, paleteRepozitorijum, vinskiPodrumiRepozitorijum);


            if (korisniciRepozitorijum.SviKorisnici().Count() == 0)
            {
                Korisnik enolog = new Korisnik("enolog", "enolog123", "Marko Markovic", TipKorisnika.GlavniEnolog);
                Korisnik kelar = new Korisnik("kelar", "kelar123", "Jovan Jovanovic", TipKorisnika.KelarMajstor);

                korisniciRepozitorijum.DodajKorisnika(enolog);
                korisniciRepozitorijum.DodajKorisnika(kelar);

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, "Kreirani inicijalni korisnici sistema");
                InicijalizujTestnePodatke(vinogradarstvoServis, vinskiPodrumiRepozitorijum, lozeRepozitorijum, loggerServis);
            }

            AutentifikacioniMeni am = new AutentifikacioniMeni(autentifikacijaServis);
            Korisnik prijavljen = new Korisnik();

            while (am.TryLogin(out prijavljen) == false)
            {
                Console.WriteLine("Pogrešno korisničko ime ili loznika. Pokušajte ponovo.");
            }

            Console.Clear();
            Console.WriteLine($"Uspešno ste prijavljeni kao {prijavljen.ImePrezime} ({prijavljen.Uloga})");
            Console.WriteLine();

            ISkladistenjeServis skladistenjeServis = prijavljen.Uloga == TipKorisnika.GlavniEnolog ? vinskiPodrumSkladistenjeServis : lokalniKelarSkladistenjeServis;

            IProdajaServis prodajaServis = new ProdajaServisi(vinaRepozitorijum, loggerServis, faktureRepozitorijum, paleteRepozitorijum, skladistenjeServis);

            OpcijeMeni meni = new OpcijeMeni(
                prijavljen,
                vinogradarstvoServis,
                proizvodnjaVinaServis,
                pakovanjeServis,
                skladistenjeServis,
                prodajaServis,
                vinskiPodrumServis,
                loggerServis

            );
            meni.PrikaziMeni();
        }
        private static void InicijalizujTestnePodatke(
            IVinogradarstvoServis vinogradarstvoServis,
            IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum,
            ILozeRepozitorijum lozeRepozitorijum,
            ILoggerServis loggerServis)
        {
            var loza1 = vinogradarstvoServis.PosadiNovuLozu("Sangiovese", "Chianti");
            loza1.FazaZrelosti = FazaZrelosti.SpremnaZaBerbu;
            lozeRepozitorijum.AzurirajLozu(loza1);

            var loza2 = vinogradarstvoServis.PosadiNovuLozu("Trebbiano", "Toscana");
            loza2.FazaZrelosti = FazaZrelosti.SpremnaZaBerbu;
            lozeRepozitorijum.AzurirajLozu(loza2);

            var loza3 = vinogradarstvoServis.PosadiNovuLozu("Merlot", "Bordeaux");
            loza3.FazaZrelosti = FazaZrelosti.SpremnaZaBerbu;
            lozeRepozitorijum.AzurirajLozu(loza3);

            var loza4 = vinogradarstvoServis.PosadiNovuLozu("Cabernet Sauvignon", "Napa Valley");
            loza4.FazaZrelosti = FazaZrelosti.Posadjena;
            lozeRepozitorijum.AzurirajLozu(loza4);

            var loza5 = vinogradarstvoServis.PosadiNovuLozu("Pinot Noir", "Burgundy");
            loza5.FazaZrelosti = FazaZrelosti.Posadjena;
            lozeRepozitorijum.AzurirajLozu(loza5);

            var podrum1 = new VinskiPodrum("Glavni Vinski Podrum", 12.5, 10);
            vinskiPodrumiRepozitorijum.DodajVinskiPodrum(podrum1);

            var podrum2 = new VinskiPodrum("Lokalni Kelar", 14.0, 5);
            vinskiPodrumiRepozitorijum.DodajVinskiPodrum(podrum2);

            loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, "Inicijalizovani testni podaci: 5 loza i 2 vinska podruma");
        }
    }
}