using NUnit.Framework;
using Moq;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using Services.BalansiranjeSeceraServisi;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class BalansiranjeSeceraServisTests
    {
        private Mock<IVinogradarstvoServis> mockVinogradarstvo;
        private Mock<ILozeRepozitorijum> mockLozeRepo;
        private Mock<IKonfiguracijaVinarije> mockKonfiguracija;
        private BalansiranjeSeceraServis servis;

        [SetUp]
        public void Setup()
        {
            mockVinogradarstvo = new Mock<IVinogradarstvoServis>();
            mockLozeRepo = new Mock<ILozeRepozitorijum>();
            mockKonfiguracija = new Mock<IKonfiguracijaVinarije>();
            mockKonfiguracija.Setup(k => k.OptimalniBrix).Returns(24.0);
            mockKonfiguracija.Setup(k => k.DefaultniRegion).Returns("Toskana");

            servis = new BalansiranjeSeceraServis(
                mockVinogradarstvo.Object,
                mockLozeRepo.Object,
                mockKonfiguracija.Object
            );
        }

        [Test]
        public void BalansirajSecer_KreiraNovuLozuZaVisokeVrednosti_Uspesno()
        {
            var loza = new Loza("Chianti", 26.0, 2025, "Toskana") { Id = 1 };
            var loze = new List<Loza> { loza };

            var balansirajucaLoza = new Loza("Chianti", 20.0, 2025, "Toskana") { Id = 2 };
            mockVinogradarstvo.Setup(v => v.PosadiNovuLozu("Chianti", "Toskana")).Returns(balansirajucaLoza);
            mockLozeRepo.Setup(r => r.AzurirajLozu(It.IsAny<Loza>())).Returns(true);

            var rezultat = servis.BalansirajSecer(loze, 24.0);

            Assert.That(rezultat.Count, Is.EqualTo(1));
            mockVinogradarstvo.Verify(v => v.PosadiNovuLozu("Chianti", "Toskana"), Times.Once);
        }

        [Test]
        public void BalansirajSecer_NeKreiraLozuZaNiskeVrednosti_Uspesno()
        {
            var loza = new Loza("Chianti", 22.0, 2025, "Toskana") { Id = 1 };
            var loze = new List<Loza> { loza };

            var rezultat = servis.BalansirajSecer(loze, 24.0);

            Assert.That(rezultat.Count, Is.EqualTo(0));
            mockVinogradarstvo.Verify(v => v.PosadiNovuLozu(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}