using NUnit.Framework;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Services.CenovneStrategije;

namespace Tests
{
    [TestFixture]
    public class CenovneStrategijeTests
    {
        [Test]
        public void StolnoVinoCenovnaStrategija_IzracunavaIspravnuCenu_Uspesno()
        {
            var strategija = new StolnoVinoCenovnaStrategija();
            var vino = new Vino("Test", KategorijaVina.StolnoVino, 0.75, 1);

            double cena = strategija.IzracunajCenu(vino);

            Assert.That(cena, Is.EqualTo(6.0));
            Assert.That(strategija.PrimenjivZa(KategorijaVina.StolnoVino), Is.True);
        }

        [Test]
        public void KvalitetnoVinoCenovnaStrategija_IzracunavaIspravnuCenu_Uspesno()
        {
            var strategija = new KvalitetnoVinoCenovnaStrategija();
            var vino = new Vino("Test", KategorijaVina.KvalitetnoVino, 0.75, 1);

            double cena = strategija.IzracunajCenu(vino);

            Assert.That(cena, Is.EqualTo(11.25));
            Assert.That(strategija.PrimenjivZa(KategorijaVina.KvalitetnoVino), Is.True);
        }

        [Test]
        public void PremijumVinoCenovnaStrategija_IzracunavaIspravnuCenu_Uspesno()
        {
            var strategija = new PremijumVinoCenovnaStrategija();
            var vino = new Vino("Test", KategorijaVina.PremijumVino, 0.75, 1);

            double cena = strategija.IzracunajCenu(vino);

            Assert.That(cena, Is.EqualTo(26.25));
            Assert.That(strategija.PrimenjivZa(KategorijaVina.PremijumVino), Is.True);
        }

        [Test]
        public void CenovnaStrategija_NePrimenjivZaDruguKategoriju_Uspesno()
        {
            var strategija = new StolnoVinoCenovnaStrategija();

            Assert.That(strategija.PrimenjivZa(KategorijaVina.PremijumVino), Is.False);
        }
    }
}