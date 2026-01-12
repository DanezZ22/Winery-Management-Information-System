using NUnit.Framework;
using Moq;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using Services.ProizvodnjaVinaServisi;


namespace Tests.Domain
{
    [TestFixture]
    public class LozaTests
    {
        [Test]
        public void Loza_KonstruktorPostavljaVrednosti_Uspesno()
        {
            string naziv = "Lara";
            double nivoSecera = 16.0;
            int godinaSadnje = 2004;
            string regionUzgoja = "Milano";

            var loza = new Loza(naziv, nivoSecera, godinaSadnje, regionUzgoja);

            Assert.That(loza.Naziv, Is.EqualTo(naziv));
            Assert.That(loza.NivoSecera, Is.EqualTo(nivoSecera));
            Assert.That(loza.GodinaSadnje, Is.EqualTo(godinaSadnje));
            Assert.That(loza.RegionUzgoja, Is.EqualTo(regionUzgoja));
            Assert.That(loza.FazaZrelosti, Is.EqualTo(FazaZrelosti.Posadjena));
        }

        [Test]
        public void Loza_ProveraDaLiNivoSeceraOstajeUDobromOpsegu_Uspesno()
        {
            string naziv = "Lara";
            double nivoSecera = 22.0;
            int godinaSadnje = 2004;
            string regionUzgoja = "Milano";

            var loza = new Loza(naziv, nivoSecera, godinaSadnje, regionUzgoja);

            Assert.That(loza.NivoSecera, Is.InRange(15.0, 28.0));
        }

        [Test]
        public void Loza_ProveraPromeneFazeZrelosti_Uspesno()
        {
            string naziv = "Lara";
            double nivoSecera = 22.0;
            int godinaSadnje = 2004;
            string regionUzgoja = "Milano";

            var loza = new Loza(naziv, nivoSecera, godinaSadnje, regionUzgoja);
            loza.FazaZrelosti = FazaZrelosti.Obrana;

            Assert.That(loza.FazaZrelosti, Is.EqualTo(FazaZrelosti.Obrana));
        }
    }
}
