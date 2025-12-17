using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaci
{
    public interface IProizvodnjaVinaServis
    {
        List<Vino> ZapocniFermentaciju(string nazivVina, KategorijaVina kategorija, int brojFlasa, double zapreminaFlase);
        List<Vino> DobijProizvedenaVina(KategorijaVina kategorija, int kolicina);
    }
}
