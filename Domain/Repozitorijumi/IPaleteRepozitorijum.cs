using Domain.Modeli.Enumeracije;
using Domain.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repozitorijumi
{
    public interface IPaleteRepozitorijum
    {
        Paleta DodajPaletu(Paleta paleta);
        Paleta PronadjiPaletuPoId(long id);
        IEnumerable<Paleta> SvePalete();
        IEnumerable<Paleta> PronadjiPaletePoStatusu(StatusPalete status);
        bool AzurirajPaletu(Paleta paleta);
        bool ObrisiPaletu(long id);
    }
}
