using Domain.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaci
{
    public interface IPakovanjeServis
    {
        Paleta PakujVino(long idVinskogPodruma, string adresaOdredista, List<long> idVina);
        bool PosaljiPaletuUPodrum(long idPalete);
    }
}
