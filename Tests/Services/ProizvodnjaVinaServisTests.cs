using NUnit.Framework;
using Moq;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using Services.ProizvodnjaVinaServisi;
using System.Collections.Generic;
using Services.BalansiranjeSeceraServisi;

namespace Tests
{
    [TestFixture]
    public class ProizvodnjaVinaServisTests
    {
        private Mock<IVinaRepozitorijum> mockVinaRepo;
        private Mock<ILozeRepozitorijum> mockLozeRepo;
        private Mock<IVinogradarstvoServis> mockVinogradarstvo;
        private Mock<ILoggerServis> mockLogger;
        private Mock<IResursKalkulatorServis> mockResursKalkulator;
        private Mock<IBalansiranjeSeceraServis> mockBalansiranje;
        private Mock<IKonfiguracijaVinarije> mockKonfiguracija;
        private ProizvodnjaVinaServis servis;

        [SetUp]
        public void Setup()
        {
            mockVinaRepo = new Mock<IVinaRepozitorijum>();
            mockLozeRepo = new Mock<ILozeRepozitorijum>();
            mockVinogradarstvo = new Mock<IVinogradarstvoServis>();
            mockLogger = new Mock<ILoggerServis>();
            mockResursKalkulator = new Mock<IResursKalkulatorServis>();
            mockBalansiranje = new Mock<IBalansiranjeSeceraServis>();
            mockKonfiguracija = new Mock<IKonfiguracijaVinarije>();

            mockKonfiguracija.Setup(k => k.OptimalniBrix).Returns(24.0);
            mockKonfiguracija.Setup(k => k.DefaultniRegion).Returns("Toskana");

            servis = new ProizvodnjaVinaServis(
                mockVinaRepo.Object,
                mockLozeRepo.Object,
                mockVinogradarstvo.Object,
                mockLogger.Object,
                mockResursKalkulator.Object,
                mockBalansiranje.Object,
                mockKonfiguracija.Object
            );
        }

        [Test]
        public void ZapocniFermentaciju_KoristitResursKalkulator_Uspesno()
        {
            var obranaLoza = new Loza("Sangiovese", 23.0, 2024, "Chianti")
            { Id = 1, FazaZrelosti = FazaZrelosti.Obrana };

            mockResursKalkulator.Setup(r => r.IzracunajPotrebanBrojLoza(1, 0.75)).Returns(1);
            mockLozeRepo.Setup(r => r.PronadjiLozePoFaziZrelosti(FazaZrelosti.Obrana))
                .Returns(new List<Loza> { obranaLoza });
            mockBalansiranje.Setup(b => b.BalansirajSecer(It.IsAny<List<Loza>>(), 24.0))
                .Returns(new List<Loza>());
            mockVinaRepo.Setup(r => r.DodajVino(It.IsAny<Vino>()))
                .Returns((Vino v) => { v.Id = 1; return v; });
            mockVinaRepo.Setup(r => r.AzurirajVino(It.IsAny<Vino>())).Returns(true);

            var rezultat = servis.ZapocniFermentaciju("Chianti", KategorijaVina.KvalitetnoVino, 1, 0.75);

            Assert.That(rezultat.Count, Is.EqualTo(1));
            mockResursKalkulator.Verify(r => r.IzracunajPotrebanBrojLoza(1, 0.75), Times.Once);
        }

        [Test]
        public void ZapocniFermentaciju_KoristitBalansiranjeSecera_Uspesno()
        {
            var obranaLoza = new Loza("TestVino", 26.0, 2025, "Toskana")
            { Id = 2, FazaZrelosti = FazaZrelosti.Obrana };
            var balansirajucaLoza = new Loza("TestVino", 22.0, 2025, "Toskana")
            { Id = 3, FazaZrelosti = FazaZrelosti.Obrana };

            mockResursKalkulator.Setup(r => r.IzracunajPotrebanBrojLoza(1, 0.75)).Returns(1);
            mockLozeRepo.Setup(r => r.PronadjiLozePoFaziZrelosti(FazaZrelosti.Obrana))
                .Returns(new List<Loza> { obranaLoza });
            mockBalansiranje.Setup(b => b.BalansirajSecer(It.IsAny<List<Loza>>(), 24.0))
                .Returns(new List<Loza> { balansirajucaLoza });
            mockVinaRepo.Setup(r => r.DodajVino(It.IsAny<Vino>()))
                .Returns((Vino v) => { v.Id = 1; return v; });
            mockVinaRepo.Setup(r => r.AzurirajVino(It.IsAny<Vino>())).Returns(true);

            var rezultat = servis.ZapocniFermentaciju("TestVino", KategorijaVina.StolnoVino, 1, 0.75);

            mockBalansiranje.Verify(b => b.BalansirajSecer(It.IsAny<List<Loza>>(), 24.0), Times.Once);
        }

        [Test]
        public void DobijProizvedenaVina_VracaVinaPoKategoriji_Uspesno()
        {
            var vina = new List<Vino>
            {
                new Vino("Vino 1", KategorijaVina.PremijumVino, 0.75, 1),
                new Vino("Vino 2", KategorijaVina.PremijumVino, 0.75, 2)
            };

            mockVinaRepo.Setup(r => r.PronadjiVinaPoKategoriji(KategorijaVina.PremijumVino)).Returns(vina);

            var rezultat = servis.DobijProizvedenaVina(KategorijaVina.PremijumVino, 2);

            Assert.That(rezultat.Count, Is.EqualTo(2));
        }
    }
}