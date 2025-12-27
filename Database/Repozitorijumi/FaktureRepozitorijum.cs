using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace Database.Repozitorijumi
{
    public class FaktureRepozitorijum : IFaktureRepozitorijum
    {
        private readonly IBazaPodataka bazaPodataka;

        public FaktureRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }



        public Faktura DodajFakturu(Faktura faktura)
        {
            try
            {
                faktura.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + bazaPodataka.Tabele.Fakture.Count;



                bazaPodataka.Tabele.Fakture.Add(faktura);
                bazaPodataka.SacuvajPromene();


                return faktura;
            }
            catch
            {

                return new Faktura();

            }
        }
        public Faktura PronadjiFakturuPoId(long id)
        {
            try
            {
                return bazaPodataka.Tabele.Fakture.FirstOrDefault(f => f.Id == id) ?? new Faktura();
            }
            catch
            {
                return new Faktura();
            }
        }
        public IEnumerable<Faktura> SveFakture()
        {
            try
            {
                return bazaPodataka.Tabele.Fakture;
            }
            catch
            {
                return new List<Faktura>();
            }
        }
        public bool AzurirajFakturu(Faktura faktura)
        {
            try
            {
                var postojecaFaktura = bazaPodataka.Tabele.Fakture.FirstOrDefault(f => f.Id == faktura.Id);
                if (postojecaFaktura != null)
                {
                    int index = bazaPodataka.Tabele.Fakture.IndexOf(postojecaFaktura);



                    bazaPodataka.Tabele.Fakture[index] = faktura;
                    bazaPodataka.SacuvajPromene();


                    return true;
                }

                return false;

            }
            catch
            {
                return false;
            }
        }
    }
}
