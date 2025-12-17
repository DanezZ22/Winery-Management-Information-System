using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repozitorijumi
{
    public interface IVinaRepozitorijum
    {
        Vino DodajVino(Vino vino);
        Vino PronadjiVinoPoId(long id);
        IEnumerable<Vino> SvaVina();
        IEnumerable<Vino> PronadjiVinaPoKategoriji(KategorijaVina kategorija);
        bool AzuzirajVino(Vino vino);
        bool ObrasniVino(long id);
    }
}
