using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaci;
using Domain.Modeli.Enumeracije;

namespace Services.LoggerServisi
{
    internal class LoggerServis : ILoggerServis
    {
        public bool EvidentirajDogadjaj(TipEvidencije tip, string poruka)
        {
            try
            {
                switch (tip)
                {
                    case TipEvidencije.INFO:
                        poruka = "INFO: " + poruka;
                        break;
                    case TipEvidencije.ERROR:
                        poruka = "ERROR: " + poruka; 
                        break;
                    case TipEvidencije.WARNING:
                        poruka = "WARNING: " + poruka;
                        break;
                }
                using var sw = new StreamWriter("log.txt", append: true);
                sw.Write(poruka);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
