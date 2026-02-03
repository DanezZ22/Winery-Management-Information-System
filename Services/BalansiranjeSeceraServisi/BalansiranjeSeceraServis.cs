using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using System.Collections.Generic;

namespace Services.BalansiranjeSeceraServisi
{
    public class BalansiranjeSeceraServis : IBalansiranjeSeceraServis
    {
        private readonly IVinogradarstvoServis vinogradarstvoServis;
        private readonly ILozeRepozitorijum lozeRepozitorijum;
        private readonly IKonfiguracijaVinarije konfiguracija;

        public BalansiranjeSeceraServis(
            IVinogradarstvoServis vinogradarstvoServis,
            ILozeRepozitorijum lozeRepozitorijum,
            IKonfiguracijaVinarije konfiguracija)
        {
            this.vinogradarstvoServis = vinogradarstvoServis;
            this.lozeRepozitorijum = lozeRepozitorijum;
            this.konfiguracija = konfiguracija;
        }

        public List<Loza> BalansirajSecer(List<Loza> loze, double ciljniBrix)
        {
            List<Loza> balansirajuceLoze = new List<Loza>();

            for (int i = 0; i < loze.Count; i++)
            {
                if (loze[i].NivoSecera > ciljniBrix)
                {
                    double razlika = loze[i].NivoSecera - ciljniBrix;
                    Loza balansirajucaLoza = vinogradarstvoServis.PosadiNovuLozu(loze[i].Naziv, konfiguracija.DefaultniRegion);

                    if (balansirajucaLoza.NivoSecera >= razlika)
                    {
                        balansirajucaLoza.NivoSecera -= razlika;
                    }
                    else
                    {
                        balansirajucaLoza.NivoSecera = ciljniBrix - razlika;
                    }

                    balansirajucaLoza.FazaZrelosti = FazaZrelosti.Obrana;
                    lozeRepozitorijum.AzurirajLozu(balansirajucaLoza);
                    balansirajuceLoze.Add(balansirajucaLoza);
                }
            }

            return balansirajuceLoze;
        }
    }
}