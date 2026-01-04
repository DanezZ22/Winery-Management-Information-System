using Domain.Modeli;
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
        bool PromeniNivoSecera(long idLoze, double procenat);
        List<Loza> OberiLoze(string nazivSorte, int brojLoza);
    }
}
