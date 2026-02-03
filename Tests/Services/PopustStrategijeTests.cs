using NUnit.Framework;
using Domain.Modeli.Enumeracije;
using Services.PopustStrategije;

namespace Tests
{
    [TestFixture]
    public class PopustStrategijeTests
    {
        [Test]
        public void DiskontPopustStrategija_PrimenjujePopust_Uspesno()
        {
            var strategija = new DiskontPopustStrategija();
            double cena = 100.0;

            double rezultat = strategija.PrimeniPopust(cena);

            Assert.That(rezultat, Is.EqualTo(85.0));
            Assert.That(strategija.PrimenjivZa(TipProdaje.DiskontPica), Is.True);
        }

        [Test]
        public void BezPopustaStrategija_NePromeniCenu_Uspesno()
        {
            var strategija = new BezPopustaStrategija();
            double cena = 100.0;

            double rezultat = strategija.PrimeniPopust(cena);

            Assert.That(rezultat, Is.EqualTo(100.0));
            Assert.That(strategija.PrimenjivZa(TipProdaje.RestoranskaProadaja), Is.True);
        }

        [Test]
        public void PopustStrategija_NePrimenjivZaDrugiTip_Uspesno()
        {
            var strategija = new DiskontPopustStrategija();

            Assert.That(strategija.PrimenjivZa(TipProdaje.RestoranskaProadaja), Is.False);
        }
    }
}