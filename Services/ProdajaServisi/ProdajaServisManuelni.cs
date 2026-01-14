using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ProdajaServisi
{
    public class ProdajaServisManuelni : IProdajaServis
    {
        private readonly IVinaRepozitorijum vinaRepozitorijum;
        private readonly ILoggerServis loggerServis;
        private readonly IFaktureRepozitorijum faktureRepozitorijum;
        private readonly ISkladistenjeServis skladistenjeServis;
        private readonly IPaleteRepozitorijum paleteRepozitorijum;

        public ProdajaServisManuelni(
            IVinaRepozitorijum vinaRepozitorijum,
            ILoggerServis loggerServis,
            IFaktureRepozitorijum faktureRepozitorijum,
            IPaleteRepozitorijum paleteRepozitorijum,
            ISkladistenjeServis skladistenjeServis)
        {
            this.vinaRepozitorijum = vinaRepozitorijum;
            this.loggerServis = loggerServis;
            this.faktureRepozitorijum = faktureRepozitorijum;
            this.paleteRepozitorijum = paleteRepozitorijum;
            this.skladistenjeServis = skladistenjeServis;
        }

        public List<Vino> DobijKatalog()
        {
            try
            {
                var katalog = vinaRepozitorijum.SvaVina().ToList();
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Katalog prikazan san {katalog.Count} vina");
                return katalog;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Neuspesno gledanje kataloga: {ex.Message}");
                return new List<Vino>();
            }
        }

        public Faktura KreirajFakturu(TipProdaje tipProdaje, NacinPlacanja nacinPlacanja, List<(long idVina, int kolicina)> stavke)
        {
            try
            {
                Faktura faktura = new Faktura(tipProdaje, nacinPlacanja);

                foreach (var stavka in stavke)
                {
                    var vino = vinaRepozitorijum.PronadjiVinoPoId(stavka.idVina);
                    if (vino.Id == 0)
                    {
                        loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Vino sa ID {stavka.idVina} nije pronadjeno");
                        continue;
                    }

                    double cena = IzracunajCenu(vino, tipProdaje);
                    StavkaFakture novaStavka = new StavkaFakture(stavka.idVina, stavka.kolicina, cena);
                    faktura.Stavke.Add(novaStavka);
                }

                int ukupnaPotrebnaVina = stavke.Sum(s => s.kolicina);
                int potrebnePalete = (int)Math.Ceiling(ukupnaPotrebnaVina / 24.0);

                var isporucenePalete = skladistenjeServis.IsporuciPalete(potrebnePalete);

                if (isporucenePalete.Count == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Nema dostupnih paleta za prodaju.");

                    Console.WriteLine("\nGREŠKA: Nema dovoljno paleta za isporuku!");
                    Console.WriteLine($"Potrebno: {potrebnePalete} paleta");
                    Console.WriteLine($"Dostupno: 0 paleta");
                    Console.WriteLine("\nMolimo vas da:");
                    Console.WriteLine("1. Proizvedete vino (Meni → Proizvodnja Vina)");
                    Console.WriteLine("2. Pakujete vino u palete (Meni → Pakovanje)");
                    Console.WriteLine("3. Pošaljete palete u podrum (Meni → Pakovanje)");

                    return new Faktura();
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Isporuceno {isporucenePalete.Count} za prodaju");

                faktura = faktureRepozitorijum.DodajFakturu(faktura);

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Kreirana faktura ID {faktura.Id} ukupan iznos: {faktura.UkupanIznos:F2} EUR");

                return faktura;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greska pri kreiranju fakture: {ex.Message}");
                return new Faktura();
            }
        }

        private double IzracunajCenu(Vino vino, TipProdaje tipProdaje)
        {
            double bazna = vino.Kategorija switch
            {
                KategorijaVina.StolnoVino => 8.0,
                KategorijaVina.KvalitetnoVino => 15.0,
                KategorijaVina.PremijumVino => 35,
                _ => 10.0
            };

            double cena = vino.Zapremina * bazna;

            if (tipProdaje == TipProdaje.DiskontPica)
            {
                cena *= 0.85;
            }

            return Math.Round(cena, 2);
        }

        public List<Faktura> PregledFaktura()
        {
            try
            {
                var fakture = faktureRepozitorijum.SveFakture().ToList();
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Vidjene su sve fakture");
                return fakture;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Neuspesno gledanje faktura");
                return new List<Faktura>();
            }
        }
    }
}