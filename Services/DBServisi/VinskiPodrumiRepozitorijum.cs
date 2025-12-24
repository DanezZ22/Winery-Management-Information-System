using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Services.DBServisi
{
    public class VinskiPodrumiRepozitorijum : IVinskiPodrumiRepozitorijum
    {
        public bool AzuzirajVinskiPodrum(VinskiPodrum podrum)
        {
            try
            {
                var postojeciVinskiPodrum = IBazaPodataka.Tabele.VinskiPodrumi.FirstOrDefault(p => p.Id == podrum.Id);
                if (postojeciVinskiPodrum != null)
                {
                    int index = IBazaPodataka.Tabele.VinskiPodrum.IndexOf(postojeciVinskiPodrum);

                    IBazaPodataka.Tabele.VinskiPodrum[index] = podrum;
                    bazePodataka.SacuvajPromene();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public VinskiPodrum DodajVinskiPodrum(VinskiPodrum podrum)
        {
            try
            {
                podrum.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + bazaPodataka.Tabele.VisnkiPodrumi.Count;

                bazaPodataka.Tabele.VinskiPodrumi.Add(podrum);
                bazaPodataka.SacuvajPromene();
                return podrum;
            }
            catch
            {
                return new VinskiPodrum();
            }
        }

        public bool ObrisiVinskiPodrum(long id)
        {
            try
            {
                var podrum = IBazaPodataka.Tabele.VinskiPodrumi.FirstOrDefault(p => p.Id == id);
                if (podrum != null)
                {
                    IBazaPodataka.Tabele.VinskiPodrumi.Remove(podrum)
                    IBazaPodataka.SacuvajPromene();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public VinskiPodrum PronadjiVinskiPodrumPoId(long id)
        {
            try
            {
                return IBazaPodataka.Tabele.VinskiPodrumi.FirstOrDefault(p => p.Id == id);
            }
            catch
            {
                return new VinskiPodrum();
            }
        }

        public IEnumerable<VinskiPodrum> SviVinskiPodrumi()
        {
            try
            {
                return IBazaPodataka.Tabele.VinskiPodrumi;
            }
            catch
            {
                return new List<VinskiPodrum>();
            }
        }
    }
}
