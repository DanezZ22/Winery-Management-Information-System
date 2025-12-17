using Domain.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repozitorijumi
{
    public interface IFaktureRepozitorijum
    {
        Faktura DodajFakturu(Faktura faktura);
        Faktura PronadjiFakturuPoId(long id);
        IEnumerable<Faktura> SveFakture();
        bool AzurirajFakturu(Faktura faktura);
    }
}
