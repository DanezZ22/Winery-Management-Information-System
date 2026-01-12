using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaci
{
    public interface IVinogradarstvoServis
    {
        Loza PosadiNovuLozu(string naziv, string regionUzgoja);
        Loza PromeniNivoSecera(long idLoze, double procenat);
        List<Loza> OberiLoze(string nazivSorte, int brojLoza);

        Loza PromeniFazuZrelosti(long idLoze, FazaZrelosti novaFaza);

        List<Loza> DobijSveLoze();

    }
}
