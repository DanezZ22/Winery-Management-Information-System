using Domain.BazaPodataka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Database
{
    public class XmlBazaPodataka : IBazaPodataka
    {
        private readonly string putanjaDoBaze;
        public TabeleBazaPodataka Tabele { get; set; }

        public XmlBazaPodataka(string putanjaDoBaze)
        {
            this.putanjaDoBaze = putanjaDoBaze;

            if (File.Exists(putanjaDoBaze))
            {
                UcitajPodatke();
            }
            else
            {
                Tabele = new TabeleBazaPodataka();
                SacuvajPromene();
            }
        }

        public bool SacuvajPromene()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TabeleBazaPodataka));
                using (StreamWriter writer = new StreamWriter(putanjaDoBaze))
                {
                    serializer.Serialize(writer, Tabele);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void UcitajPodatke()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TabeleBazaPodataka));
                using (StreamReader reader = new StreamReader(putanjaDoBaze))
                {
                    Tabele = (TabeleBazaPodataka)serializer.Deserialize(reader);
                }
            }
            catch
            {
                Tabele = new TabeleBazaPodataka();
            }
        }
    }
}
