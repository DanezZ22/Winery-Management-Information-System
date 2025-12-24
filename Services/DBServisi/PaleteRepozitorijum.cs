using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;

namespace Services.DBServisi
{
    public class PaleteRepozitorijum : IPaleteRepozitorijum
    {


        public bool AzurirajPaletu(Paleta paleta)
        {
            try
            {
                var postojecaPaleta = IBazaPodataka.Tabele.Palete.FirstOrDefault(p => p.Id == paleta.Id);
                if(postojecaPaleta != null)
                {
                    int index = IBazaPodataka.Tabele.Palete.IndexOf(postojecaPaleta);

                    IBazaPodataka.Tabele.Palete[index] = paleta;
                    bazePodataka.SacuvajPromene();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public Paleta DodajPaletu(Paleta paleta)
        {
            try
            {
                paleta.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + bazaPodataka.Tabele.Palete.Count;
                paleta.Sifra = $"PAL-{DateTime.Now.Year}-{paleta.Id}";



                bazaPodataka.Tabele.Palete.Add(paleta);
                bazaPodataka.SacuvajPromene();
                return paleta;
            }
            catch
            {
                return new Paleta();
            }
        }

        public bool ObrisiPaletu(long id)
        {
            try
            {
                var paleta = IBazaPodataka.Tabele.Palete.FirstOrDefault(p  => p.Id == id);
                if (paleta != null)
                {
                    IBazaPodataka.Tabele.Palete.Remove(paleta)
                    IBazaPodataka.SacuvajPromene();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Paleta> PronadjiPaletePoStatusu(StatusPalete status)
        {
            try
            {
                return IBazaPodataka.Tabele.Palete.Where(p => p.Status == status);
            }
            catch
            {
                return new List<Paleta>();
            }
        }

        public Paleta PronadjiPaletuPoId(long id)
        {
            try
            {
                return IBazaPodataka.Tabele.Palete.FirstOrDefault(p => p.Id == id);
            }
            catch
            {
                return new Paleta();
            }
        }

        public IEnumerable<Paleta> SvePalete()
        {
            try
            {
                return IBazaPodataka.Tabele.Palete;
            }
            catch
            {
                return new List<Paleta>();
            }
        }
    }
}
