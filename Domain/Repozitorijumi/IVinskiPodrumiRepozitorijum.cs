using Domain.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repozitorijumi
{
    public interface IVinskiPodrumiRepozitorijum
    {
        VinskiPodrum DodajVinskiPodrum(VinskiPodrum podrum);
        VinskiPodrum PronadjiVinskiPodrumPoId(long id);
        IEnumerable<VinskiPodrum> SviVinskiPodrumi();
        bool AzurirajVinskiPodrum(VinskiPodrum podrum);
        bool ObrisiVinskiPodrum(long id);
    }
}
