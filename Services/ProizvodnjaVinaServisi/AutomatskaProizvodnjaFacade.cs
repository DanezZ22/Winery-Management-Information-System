using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using System.Collections.Generic;
using System.Linq;

namespace Services.AutomatskaProizvodnjaServisi
{
    public class AutomatskaProizvodnjaFacade : IAutomatskaProizvodnjaFacade
    {
        private readonly IVinaRepozitorijum vinaRepozitorijum;
        private readonly IProizvodnjaVinaServis proizvodnjaVinaServis;
        private readonly IPakovanjeServis pakovanjeServis;
        private readonly IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum;
        private readonly IPaleteRepozitorijum paleteRepozitorijum;
        private readonly IResursKalkulatorServis resursKalkulator;
        private readonly ILoggerServis loggerServis;
        private readonly IKonfiguracijaVinarije konfiguracija;

        public AutomatskaProizvodnjaFacade(
            IVinaRepozitorijum vinaRepozitorijum,
            IProizvodnjaVinaServis proizvodnjaVinaServis,
            IPakovanjeServis pakovanjeServis,
            IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum,
            IPaleteRepozitorijum paleteRepozitorijum,
            IResursKalkulatorServis resursKalkulator,
            ILoggerServis loggerServis,
            IKonfiguracijaVinarije konfiguracija)
        {
            this.vinaRepozitorijum = vinaRepozitorijum;
            this.proizvodnjaVinaServis = proizvodnjaVinaServis;
            this.pakovanjeServis = pakovanjeServis;
            this.vinskiPodrumiRepozitorijum = vinskiPodrumiRepozitorijum;
            this.paleteRepozitorijum = paleteRepozitorijum;
            this.resursKalkulator = resursKalkulator;
            this.loggerServis = loggerServis;
            this.konfiguracija = konfiguracija;
        }

        public void ObezediVina(Dictionary<KategorijaVina, int> potrebnoPoKategoriji, List<(long idVina, int kolicina)> stavke)
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

        public void ObezediPalete(int ukupnaPotrebnaVina)
        {
            int potrebnePalete = resursKalkulator.IzracunajPotrebanBrojPaleta(ukupnaPotrebnaVina);
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
                        .Take(konfiguracija.VinaPopaleti)
                        .Select(v => v.Id)
                        .ToList();

                    if (vinaZaPakovanje.Count > 0)
                    {
                        var novaPaleta = pakovanjeServis.PakujVino(podrum.Id, konfiguracija.AutomatskaAdresaIsporuke, vinaZaPakovanje);
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
    }
}