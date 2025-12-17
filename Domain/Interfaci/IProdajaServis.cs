using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaci
{
    public interface IProdajaServis
    {
        List<Vino> DobijKatalog();
        Faktura KreirajFakturu(TipProdaje tipProdaje, NacinPlacanja nacinPlacanja, List<(long idVina, int kolicina)> stavke);
        List<Faktura> PregledFaktura();
    }
}
