using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.ProizvodnjaVinaServisi
{
    public class ProizvodnjaVinaServis : IProizvodnjaVinaServis
    {
        private readonly IVinaRepozitorijum vinaRepozitorijum;
        private readonly ILozeRepozitorijum lozeRepozitorijum;
        private readonly IVinogradarstvoServis vinogradarstvoServis;
        private readonly ILoggerServis loggerServis;
        private readonly IResursKalkulatorServis resursKalkulator;
        private readonly IBalansiranjeSeceraServis balansiranjeSecera;
        private readonly IKonfiguracijaVinarije konfiguracija;

        public ProizvodnjaVinaServis(
            IVinaRepozitorijum vinaRepozitorijum,
            ILozeRepozitorijum lozeRepozitorijum,
            IVinogradarstvoServis vinogradarstvoServis,
            ILoggerServis loggerServis,
            IResursKalkulatorServis resursKalkulator,
            IBalansiranjeSeceraServis balansiranjeSecera,
            IKonfiguracijaVinarije konfiguracija)
        {
            this.vinaRepozitorijum = vinaRepozitorijum;
            this.lozeRepozitorijum = lozeRepozitorijum;
            this.vinogradarstvoServis = vinogradarstvoServis;
            this.loggerServis = loggerServis;
            this.resursKalkulator = resursKalkulator;
            this.balansiranjeSecera = balansiranjeSecera;
            this.konfiguracija = konfiguracija;
        }

        public List<Vino> ZapocniFermentaciju(string nazivVina, KategorijaVina kategorijaVina, int brojFlasa, double zapreminaFlase)
        {
            try
            {
                int potrebnoBrojLoza = resursKalkulator.IzracunajPotrebanBrojLoza(brojFlasa, zapreminaFlase);
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Fermentacija: potrebno {potrebnoBrojLoza} loza za {brojFlasa} flasa");

                List<Loza> obraneLoze = lozeRepozitorijum.PronadjiLozePoFaziZrelosti(FazaZrelosti.Obrana).ToList();

                if (obraneLoze.Count < potrebnoBrojLoza)
                {
                    int nedostaje = potrebnoBrojLoza - obraneLoze.Count;
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Nedostaje {nedostaje} loza, pokrecem automatsko sadenje");

                    for (int i = 0; i < nedostaje; i++)
                    {
                        Loza novaLoza = vinogradarstvoServis.PosadiNovuLozu(nazivVina, konfiguracija.DefaultniRegion);
                        novaLoza.FazaZrelosti = FazaZrelosti.SpremnaZaBerbu;
                        lozeRepozitorijum.AzurirajLozu(novaLoza);

                        var obereneLoze = vinogradarstvoServis.OberiLoze(nazivVina, 1);
                        if (obereneLoze.Count > 0)
                        {
                            obraneLoze.Add(obereneLoze[0]);
                        }
                    }
                }

                List<Loza> balansirajuceLoze = balansiranjeSecera.BalansirajSecer(obraneLoze, konfiguracija.OptimalniBrix);
                obraneLoze.AddRange(balansirajuceLoze);

                List<Vino> proizvedenaVina = new List<Vino>();
                for (int i = 0; i < brojFlasa; i++)
                {
                    Vino vino = new Vino(nazivVina, kategorijaVina, zapreminaFlase, obraneLoze[i % obraneLoze.Count].Id);
                    vino = vinaRepozitorijum.DodajVino(vino);
                    vino.SifraSerije = $"VN-{DateTime.Now.Year}-{vino.Id}";
                    vino.DatumFlasiranja = DateTime.Now;
                    vinaRepozitorijum.AzurirajVino(vino);
                    proizvedenaVina.Add(vino);
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Fermentacija zavrsena: proizvedeno {proizvedenaVina.Count} flasa");
                return proizvedenaVina;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greska pri fermentaciji: {ex.Message}");
                return new List<Vino>();
            }
        }

        public List<Vino> DobijProizvedenaVina(KategorijaVina kategorijaVina, int kolicina)
        {
            try
            {
                var vina = vinaRepozitorijum.PronadjiVinaPoKategoriji(kategorijaVina).Take(kolicina).ToList();
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Dobijeno {vina.Count} vina kategorije {kategorijaVina}");
                return vina;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greska pri dobavljanju vina: {ex.Message}");
                return new List<Vino>();
            }
        }
    }
}