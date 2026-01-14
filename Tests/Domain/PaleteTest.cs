using NUnit.Framework;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;

namespace Tests.Domain
{
    [TestFixture]
    public class PaletaTests
    {
        [Test]
        public void Paleta_KonstruktorPostavljaVrednosti_Uspesno()
        {
            string adresa = "Milano";
            long idPodruma = 999;

            var paleta = new Paleta(adresa, idPodruma);

            Assert.That(paleta.AdresaOdredista, Is.EqualTo(adresa));
            Assert.That(paleta.IdVinskogPodruma, Is.EqualTo(idPodruma));
            Assert.That(paleta.Status, Is.EqualTo(StatusPalete.Upakovana));
            Assert.That(paleta.IdVina, Is.Not.Null);
        }

        [Test]
        public void Paleta_DodavanjeVinaUListu_Uspesno()
        {
            var paleta = new Paleta("Roma", 1);

            paleta.IdVina.Add(100);
            paleta.IdVina.Add(101);
            paleta.IdVina.Add(102);

            Assert.That(paleta.IdVina.Count, Is.EqualTo(3));
            Assert.That(paleta.IdVina, Does.Contain(101));
        }

        [Test]
        public void Paleta_PromenaStatusa_Uspesno()
        {
            var paleta = new Paleta("Firenze", 2);
            Assert.That(paleta.Status, Is.EqualTo(StatusPalete.Upakovana));

            paleta.Status = StatusPalete.Otpremljena;

            Assert.That(paleta.Status, Is.EqualTo(StatusPalete.Otpremljena));
        }
    }
}