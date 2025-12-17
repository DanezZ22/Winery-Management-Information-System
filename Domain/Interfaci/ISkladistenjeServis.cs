using Domain.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaci
{
    public interface ISkladistenjeServis
    {
        List<Paleta> IsporuciPalete(int brojPaleta);
    }
}
