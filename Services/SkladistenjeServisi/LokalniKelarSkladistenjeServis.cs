using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using Services.LoggerServisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SkladistenjeServisi
{
    public class LokalniKelarSkladistenjeServis : ISkladistenjeServis
    {

        private readonly ILoggerServis loggerServis;
        private readonly IPaleteRepozitorijum paleteRepozitorijum;
        private readonly IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum;

        public LokalniKelarSkladistenjeServis(ILoggerServis loggerServis, IPaleteRepozitorijum paleteRepozitorijum, IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum)
        {
            this.loggerServis = loggerServis;
            this.paleteRepozitorijum = paleteRepozitorijum;
            this.vinskiPodrumiRepozitorijum = vinskiPodrumiRepozitorijum;
        }


        public List<Paleta> IsporuciPalete(int brojPaleta)
        {
            try
            {
                int maxPaletaPoIsporuci = 2;
                if (brojPaleta > maxPaletaPoIsporuci)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Poslato vise od {maxPaletaPoIsporuci} palete");
                    brojPaleta = maxPaletaPoIsporuci;
                }

                var dostupnePalete = paleteRepozitorijum.PronadjiPaletePoStatusu(StatusPalete.Otpremljena)
                    .Take(brojPaleta)
                    .ToList();

                if (dostupnePalete.Count == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Nema otpremljenih paleta");
                    return new List<Paleta>();
                }

                foreach (var paleta in dostupnePalete)
                {
                    Thread.Sleep(1800);
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Pripremljena paleta {paleta.Sifra}");
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Palete su isporucene");

                return dostupnePalete;


            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Neuspesno skladistenje: {ex.Message}");
                return new List<Paleta>();
            }
        }
    }
}
