using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;

namespace Services.VinogradarstvoServisi
{
    public class VinogradarstvoServis : IVinogradarstvoServis
    {

        private readonly ILozeRepozitorijum lozeRepozitorijum;
        private readonly ILoggerServis loggerServis;
        private readonly Random random;


        public VinogradarstvoServis(ILozeRepozitorijum lozeRepozitorijum, ILoggerServis loggerServis)
        {
            this.lozeRepozitorijum = lozeRepozitorijum;
            this.loggerServis = loggerServis;
            this.random = new Random();
        }

        public Loza PosadiNovuLozu(string naziv, string regionUzgoja)
        {
            try
            {
                double nivoSecera = Math.Round(random.NextDouble() * (28.0 - 15.0) + 15.0, 2);

                Loza novaLoza = new Loza(naziv, nivoSecera, DateTime.UtcNow.Year, regionUzgoja);

                lozeRepozitorijum.DodajLozu(novaLoza);

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Uspesno evidentirano sađenje sa nivoom secera {nivoSecera} nove loze {naziv} u {regionUzgoja}");

                return novaLoza;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greška pri sadjenju: {ex.Message}");
                return new Loza();
            }
        }


        public Loza PromeniNivoSecera(long idLoze, double procenat)
        {
            try
            {
                Loza loza = lozeRepozitorijum.PronadjiLozuPoId(idLoze);
                if(loza.Id == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Loza sa ID {idLoze} nije pronadjena");
                    return new Loza();
                }

                double stariNivo = loza.NivoSecera;
                double promena = loza.NivoSecera * (procenat / 100.0);

                loza.NivoSecera += loza.NivoSecera * (procenat / 100);
                loza.NivoSecera = Math.Max(15.0, Math.Min(28.0, loza.NivoSecera));
                loza.NivoSecera = Math.Round(loza.NivoSecera, 2);

                lozeRepozitorijum.AzurirajLozu(loza);

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Promenjen nivo secera za lozu ID {idLoze} sa {stariNivo} na {loza.NivoSecera} Brix");
                return loza;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greška pri promeni nivoa šećera: {ex.Message}");
                return new Loza() ;
            }
        }


        public Loza PromeniFazuZrelosti(long idLoze, FazaZrelosti novaFaza)
        {
            try
            {
                var loza = lozeRepozitorijum.PronadjiLozuPoId(idLoze);

                if (loza.Id == 0)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING,
                        $"Loza sa ID {idLoze} nije pronađena");
                    return new Loza();
                }

                var staraFaza = loza.FazaZrelosti;
                loza.FazaZrelosti = novaFaza;

                lozeRepozitorijum.AzurirajLozu(loza);

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,
                    $"Faza zrelosti loze {idLoze} promenjena sa {staraFaza} na {novaFaza}");

                return loza;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR,
                    $"Greška pri promeni faze: {ex.Message}");
                return new Loza();
            }
        }


        public List<Loza> DobijSveLoze()
        {
            try
            {
                var loze = lozeRepozitorijum.SveLoze().ToList();
                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Prikazano {loze.Count} loza");
                return loze;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Greška pri dobijanju loza: {ex.Message}");
                return new List<Loza>();
            }
        }

        public List<Loza> OberiLoze(string nazivSorte, int brojLoza)
        {
            try
            {
                var spremneLoze = lozeRepozitorijum.PronadjiLozePoNazivu(nazivSorte)
                    .Where(l => l.FazaZrelosti == FazaZrelosti.SpremnaZaBerbu)
                    .Take(brojLoza)
                    .ToList();

                if (spremneLoze.Count < brojLoza)
                {
                    loggerServis.EvidentirajDogadjaj(TipEvidencije.WARNING, $"Nedovoljno spremnih loza sorte {nazivSorte}. Trazeno: {brojLoza}, dostupno: {spremneLoze.Count}");
                    return new List<Loza>();
                }

                foreach(var loza in spremneLoze)
                {
                    loza.FazaZrelosti = FazaZrelosti.Obrana;
                    lozeRepozitorijum.AzurirajLozu(loza);
                }

                loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Obrano {brojLoza} loza sorte {nazivSorte}.");

                return spremneLoze;
            }
            catch (Exception ex)
            {
                loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR, $"Neuspešno branje:{ex.Message}");
                return new List<Loza>();
            }
        }

    }

}