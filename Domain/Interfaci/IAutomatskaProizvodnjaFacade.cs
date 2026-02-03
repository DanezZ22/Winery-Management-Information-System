using Domain.Modeli.Enumeracije;
using System.Collections.Generic;

namespace Domain.Interfaci
{
    public interface IAutomatskaProizvodnjaFacade
    {
        void ObezediVina(Dictionary<KategorijaVina, int> potrebnoPoKategoriji, List<(long idVina, int kolicina)> stavke);
        void ObezediPalete(int ukupnaPotrebnaVina);
    }
}