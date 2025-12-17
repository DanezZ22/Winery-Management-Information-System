using Domain.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaci
{
    public interface IAutentifikacijaServis
    {
        (bool, Korisnik) Prijava(string korisnickoIme, string loznika);
    }
}
