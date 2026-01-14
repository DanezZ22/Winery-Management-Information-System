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
    public class ProdajaServisAutomatski : IProdajaServis
    {
        private const int VINA_PO_PALETI = 24;
        private const string ADRESA_AUTOMATSKE_ISPORUKE = "Automatska isporuka";

        private readonly IVinaRepozitorijum vinaRepozitorijum;
        private readonly ILoggerServis loggerServis;
        private readonly IFaktureRepozitorijum faktureRepozitorijum;
        private readonly ISkladistenjeServis skladistenjeServis;
        private readonly IPaleteRepozitorijum paleteRepozitorijum;
        private readonly IProizvodnjaVinaServis proizvodnjaVinaServis;
        private readonly IPakovanjeServis pakovanjeServis;
        private readonly IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum;

        public ProdajaServisAutomatski(
            IVinaRepozitorijum vinaRepozitorijum,
            ILoggerServis loggerServis,
            IFaktureRepozitorijum faktureRepozitorijum,
            IPaleteRepozitorijum paleteRepozitorijum,
            ISkladistenjeServis skladistenjeServis,
            IProizvodnjaVinaServis proizvodnjaVinaServis,
            IPakovanjeServis pakovanjeServis,
            IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum)
        {
            this.vinaRepozitorijum = vinaRepozitorijum;
            this.loggerServis = loggerServis;
            this.faktureRepozitorijum = faktureRepozitorijum;
            this.paleteRepozitorijum = paleteRepozitorijum;
            this.skladistenjeServis = skladistenjeServis;
            this.proizvodnjaVinaServis = proizvodnjaVinaServis;
            this.pakovanjeServis = pakovanjeServis;
            this.vinskiPodrumiRepozitorijum = vinskiPodrumiRepozitorijum;
        }

        public List<Vino> DobijKatalog()
        {
            try
            {
                var katalog = vinaRepozitorijum.SvaVina().ToList();
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Katalog prikazan sa {katalog.Count} vina");
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
                Dictionary<KategorijaVina, int> potrebnoPoKategoriji = new Dictionary<KategorijaVina, int>();

                DodajStavkeUFakturu(faktura, stavke, potrebnoPoKategoriji);
                ObezediProizvodnju(stavke, potrebnoPoKategoriji);

                int ukupnaPotrebnaVina = stavke.Sum(s => s.kolicina);
                ObezediPakovanje(ukupnaPotrebnaVina);

                int potrebnePalete = (int)Math.Ceiling(ukupnaPotrebnaVina / (double)VINA_PO_PALETI);
                var isporucenePalete = skladistenjeServis.IsporuciPalete(potrebnePalete);

                if (isporucenePalete.Count == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, "Nema dostupnih paleta za prodaju.");
                    return new Faktura();
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Isporuceno {isporucenePalete.Count} paleta za prodaju");

                ObrisiProdataVina(stavke);

                faktura = faktureRepozitorijum.DodajFakturu(faktura);
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Kreirana faktura ID {faktura.Id}, iznos: {faktura.UkupanIznos:F2} EUR");

                return faktura;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greska pri kreiranju fakture: {ex.Message}");
                return new Faktura();
            }
        }

        private void DodajStavkeUFakturu(Faktura faktura, List<(long idVina, int kolicina)> stavke, Dictionary<KategorijaVina, int> potrebnoPoKategoriji)
        {
            foreach (var stavka in stavke)
            {
                var vino = vinaRepozitorijum.PronadjiVinoPoId(stavka.idVina);
                if (vino.Id == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Vino sa ID {stavka.idVina} nije pronadjeno");
                    continue;
                }

                if (!potrebnoPoKategoriji.ContainsKey(vino.Kategorija))
                {
                    potrebnoPoKategoriji[vino.Kategorija] = 0;
                }
                potrebnoPoKategoriji[vino.Kategorija] += stavka.kolicina;

                double cena = IzracunajCenu(vino, faktura.TipProdaje);
                StavkaFakture novaStavka = new StavkaFakture(stavka.idVina, stavka.kolicina, cena);
                faktura.Stavke.Add(novaStavka);
            }
        }

        private void ObezediProizvodnju(List<(long idVina, int kolicina)> stavke, Dictionary<KategorijaVina, int> potrebnoPoKategoriji)
        {
            foreach (var kategorija in potrebnoPoKategoriji.Keys)
            {
                var dostupnaVina = vinaRepozitorijum.PronadjiVinaPoKategoriji(kategorija).Count();
                int potrebno = potrebnoPoKategoriji[kategorija];

                if (dostupnaVina < potrebno)
                {
                    int nedostaje = potrebno - dostupnaVina;
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,
                        $"Automatska proizvodnja: nedostaje {nedostaje} vina kategorije {kategorija}");

                    var primerVino = stavke
                        .Select(s => vinaRepozitorijum.PronadjiVinoPoId(s.idVina))
                        .FirstOrDefault(v => v.Kategorija == kategorija && v.Id != 0);

                    if (primerVino != null)
                    {
                        proizvodnjaVinaServis.ZapocniFermentaciju(
                            primerVino.Naziv,
                            primerVino.Kategorija,
                            nedostaje,
                            primerVino.Zapremina
                        );
                    }
                }
            }
        }

        private void ObezediPakovanje(int ukupnaPotrebnaVina)
        {
            int potrebnePalete = (int)Math.Ceiling(ukupnaPotrebnaVina / (double)VINA_PO_PALETI);
            var dostupnePalete = paleteRepozitorijum.PronadjiPaletePoStatusu(StatusPalete.Otpremljena).Count();

            if (dostupnePalete < potrebnePalete)
            {
                int nedostaje = potrebnePalete - dostupnePalete;
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,
                    $"Automatsko pakovanje: nedostaje {nedostaje} paleta");

                var podrum = vinskiPodrumiRepozitorijum.SviVinskiPodrumi().FirstOrDefault();
                if (podrum == null)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Nema dostupnih vinskih podruma");
                    return;
                }

                for (int i = 0; i < nedostaje; i++)
                {
                    var vinaZaPakovanje = vinaRepozitorijum.SvaVina()
                        .Take(VINA_PO_PALETI)
                        .Select(v => v.Id)
                        .ToList();

                    if (vinaZaPakovanje.Count > 0)
                    {
                        var novaPaleta = pakovanjeServis.PakujVino(podrum.Id, ADRESA_AUTOMATSKE_ISPORUKE, vinaZaPakovanje);
                        if (novaPaleta.Id != 0)
                        {
                            pakovanjeServis.PosaljiPaletuUPodrum(novaPaleta.Id);
                            loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,
                                $"Automatski kreirana i otpremljena paleta {novaPaleta.Sifra}");
                        }
                    }
                }
            }
        }

        private void ObrisiProdataVina(List<(long idVina, int kolicina)> stavke)
        {
            List<long> vinaNaBrisanje = new List<long>();

            foreach (var stavka in stavke)
            {
                var vino = vinaRepozitorijum.PronadjiVinoPoId(stavka.idVina);
                if (vino.Id == 0) continue;

                var vinaZaBrisanje = vinaRepozitorijum
                    .PronadjiVinaPoKategoriji(vino.Kategorija)
                    .Take(stavka.kolicina)
                    .Select(v => v.Id)
                    .ToList();

                vinaNaBrisanje.AddRange(vinaZaBrisanje);
            }

            foreach (var idVina in vinaNaBrisanje)
            {
                vinaRepozitorijum.ObrisiVino(idVina);
            }

            if (vinaNaBrisanje.Count > 0)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Prodato i obrisano {vinaNaBrisanje.Count} vina");
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
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, "Prikazane sve fakture");
                return fakture;
            }
            catch
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, "Neuspesno gledanje faktura");
                return new List<Faktura>();
            }
        }
    }
}