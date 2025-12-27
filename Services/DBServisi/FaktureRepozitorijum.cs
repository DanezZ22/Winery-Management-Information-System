using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace Services.DBServisi
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
                faktura.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + bazaPodataka.Tabele.Faktura.Count;



                bazaPodataka.Tabele.Faktura.Add(faktura);
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
                return bazaPodataka.Tabele.Faktura.FirstOrDefault(f => f.Id == id) ?? new Faktura();
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
                return bazaPodataka.Tabele.Faktura;
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
                var postojecaFaktura = bazaPodataka.Tabele.Faktura.FirstOrDefault(f => f.Id == faktura.Id);
                if (postojecaFaktura != null)
                {
                    int index = bazaPodataka.Tabele.Faktura.IndexOf(postojecaFaktura);



                    bazaPodataka.Tabele.Faktura[index] = faktura;
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
