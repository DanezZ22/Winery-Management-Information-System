using Domain.Interfaci;

namespace Services.KonfiguracijaServisi
{
    public class VinarijaKonfiguracija : IKonfiguracijaVinarije
    {
        public string DefaultniRegion => "Toskana";
        public string AutomatskaAdresaIsporuke => "Automatska isporuka";
        public double PrinosPoLozi => 1.2;
        public int VinaPopaleti => 24;
        public double OptimalniBrix => 24.0;
    }
}