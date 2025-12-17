using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repozitorijumi
{
    public interface ILozeRepozitorijum
    {
        Loza DodajLozu(Loza loza);
        Loza PronadjiLozuPoId(long id);
        IEnumerable<Loza> SveLoze();
        IEnumerable<Loza> PronadjiLozePoNazivu(string naziv);
        IEnumerable<Loza> PronadjiLozePoFaziZrelosti(FazaZrelosti faza);
        bool AzurirajLozu(Loza loza);
        bool ObrisiLozu(long id);
    }
}
