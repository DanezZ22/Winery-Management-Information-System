using NUnit.Framework;
using Moq;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using Services.ProizvodnjaVinaServisi;
using System.Collections.Generic;

namespace Tests.Services
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
            mockKonfiguracija.Setup(k => k.PrinosPoLozi).Returns(1.2);

            mockResursKalkulator.Setup(r => r.IzracunajPotrebanBrojLoza(1, 0.75)).Returns(1);

            mockBalansiranje.Setup(b => b.BalansirajSecer(It.IsAny<List<Loza>>(), It.IsAny<double>()))
                .Returns(new List<Loza>());

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
        public void ZapocniFermentaciju_RacunaPotrebanBrojLoza_Uspesno()
        {
            var obranaLoza = new Loza("Sangiovese", 23.0, 2024, "Chianti")
            { Id = 1, FazaZrelosti = FazaZrelosti.Obrana };

            mockLozeRepo.Setup(r => r.PronadjiLozePoFaziZrelosti(FazaZrelosti.Obrana))
                .Returns(new List<Loza> { obranaLoza });
            mockVinaRepo.Setup(r => r.DodajVino(It.IsAny<Vino>()))
                .Returns((Vino v) => { v.Id = 1; return v; });
            mockVinaRepo.Setup(r => r.AzurirajVino(It.IsAny<Vino>())).Returns(true);

            var rezultat = servis.ZapocniFermentaciju("Chianti", KategorijaVina.KvalitetnoVino, 1, 0.75);

            Assert.That(rezultat.Count, Is.EqualTo(1));
            mockVinaRepo.Verify(r => r.DodajVino(It.IsAny<Vino>()), Times.Once);
            mockResursKalkulator.Verify(r => r.IzracunajPotrebanBrojLoza(1, 0.75), Times.Once);
        }

        [Test]
        public void ZapocniFermentaciju_AutomatskiSadiLozuAkoNedostaje_Uspesno()
        {
            var novaLoza = new Loza("TestVino", 20.0, 2025, "Toskana")
            { Id = 2, FazaZrelosti = FazaZrelosti.Posadjena };

            mockResursKalkulator.Setup(r => r.IzracunajPotrebanBrojLoza(1, 0.75)).Returns(1);
            mockLozeRepo.Setup(r => r.PronadjiLozePoFaziZrelosti(FazaZrelosti.Obrana))
                .Returns(new List<Loza>());
            mockLozeRepo.Setup(r => r.AzurirajLozu(It.IsAny<Loza>())).Returns(true);
            mockVinogradarstvo.Setup(v => v.PosadiNovuLozu("TestVino", "Toskana")).Returns(novaLoza);
            mockVinogradarstvo.Setup(v => v.OberiLoze("TestVino", 1))
                .Returns(new List<Loza> { novaLoza });
            mockVinaRepo.Setup(r => r.DodajVino(It.IsAny<Vino>()))
                .Returns((Vino v) => { v.Id = 1; return v; });
            mockVinaRepo.Setup(r => r.AzurirajVino(It.IsAny<Vino>())).Returns(true);

            var rezultat = servis.ZapocniFermentaciju("TestVino", KategorijaVina.StolnoVino, 1, 0.75);

            mockVinogradarstvo.Verify(v => v.PosadiNovuLozu("TestVino", "Toskana"), Times.Once);
            mockVinogradarstvo.Verify(v => v.OberiLoze("TestVino", 1), Times.Once);
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
            Assert.That(rezultat.All(v => v.Kategorija == KategorijaVina.PremijumVino), Is.True);
        }
    }
}