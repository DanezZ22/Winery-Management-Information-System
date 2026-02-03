using NUnit.Framework;
using Moq;
using Domain.Interfaci;
using Services.ResursKalkulatorServisi;

namespace Tests
{
    [TestFixture]
    public class ResursKalkulatorServisTests
    {
        private Mock<IKonfiguracijaVinarije> mockKonfiguracija;
        private ResursKalkulatorServis servis;

        [SetUp]
        public void Setup()
        {
            mockKonfiguracija = new Mock<IKonfiguracijaVinarije>();
            mockKonfiguracija.Setup(k => k.PrinosPoLozi).Returns(1.2);
            mockKonfiguracija.Setup(k => k.VinaPopaleti).Returns(24);

            servis = new ResursKalkulatorServis(mockKonfiguracija.Object);
        }

        [Test]
        public void IzracunajPotrebanBrojLoza_VracaTacanBroj_Uspesno()
        {
            int rezultat = servis.IzracunajPotrebanBrojLoza(100, 0.75);

            Assert.That(rezultat, Is.EqualTo(63));
        }

        [Test]
        public void IzracunajPotrebanBrojLoza_ZaokruziNaViše_Uspesno()
        {
            int rezultat = servis.IzracunajPotrebanBrojLoza(50, 0.75);

            Assert.That(rezultat, Is.EqualTo(32));
        }

        [Test]
        public void IzracunajPotrebanBrojPaleta_VracaTacanBroj_Uspesno()
        {
            int rezultat = servis.IzracunajPotrebanBrojPaleta(100);

            Assert.That(rezultat, Is.EqualTo(5));
        }

        [Test]
        public void IzracunajPotrebanBrojPaleta_ZaokruziNaViše_Uspesno()
        {
            int rezultat = servis.IzracunajPotrebanBrojPaleta(25);

            Assert.That(rezultat, Is.EqualTo(2));
        }
    }
}