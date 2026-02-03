using Domain.Modeli;
using System.Collections.Generic;

namespace Domain.Interfaci
{
    public interface IBalansiranjeSeceraServis
    {
        List<Loza> BalansirajSecer(List<Loza> loze, double ciljniBrix);
    }
}