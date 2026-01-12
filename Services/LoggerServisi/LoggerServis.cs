using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaci;
using Domain.Modeli.Enumeracije;

namespace Services.LoggerServisi
{
    public class LoggerServis : ILoggerServis
    {

        private readonly string putanjaDoLoga;

        public LoggerServis(string putanjaDoLoga)
        {
            this.putanjaDoLoga = putanjaDoLoga;

            if(!File.Exists(putanjaDoLoga))
            {
                File.Create(putanjaDoLoga).Close();
            }
                
         }

        public bool EvidentirajDogadjaj(TipEvidencije tip, string poruka)
        {
            try
            {
                string vreme = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                string logPoruka = $"[{vreme}] [{tip}] {poruka}";

                File.AppendAllText(putanjaDoLoga, logPoruka + Environment.NewLine);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
