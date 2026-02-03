using Domain.Interfaci;
using System;

namespace Services.ResursKalkulatorServisi
{
    public class ResursKalkulatorServis : IResursKalkulatorServis
    {
        private readonly IKonfiguracijaVinarije konfiguracija;

        public ResursKalkulatorServis(IKonfiguracijaVinarije konfiguracija)
        {
            this.konfiguracija = konfiguracija;
        }

        public int IzracunajPotrebanBrojLoza(int brojFlasa, double zapreminaFlase)
        {
            double potrebnaKolicinaVina = brojFlasa * zapreminaFlase;
            return (int)Math.Ceiling(potrebnaKolicinaVina / konfiguracija.PrinosPoLozi);
        }

        public int IzracunajPotrebanBrojPaleta(int ukupnoVina)
        {
            return (int)Math.Ceiling(ukupnoVina / (double)konfiguracija.VinaPopaleti);
        }
    }
}