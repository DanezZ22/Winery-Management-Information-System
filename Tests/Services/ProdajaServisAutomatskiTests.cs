using NUnit.Framework;
using Moq;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using Services.ProdajaServisi;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Services
{
    [TestFixture]
    public class ProdajaServisAutomatskiTests
    {
        private Mock<IVinaRepozitorijum> mockVinaRepo;
        private Mock<ILoggerServis> mockLogger;
        private Mock<IFaktureRepozitorijum> mockFaktureRepo;
        private Mock<IPaleteRepozitorijum> mockPaleteRepo;
        private Mock<ISkladistenjeServis> mockSkladistenje;
        private Mock<IProizvodnjaVinaServis> mockProizvodnja;
        private Mock<IPakovanjeServis> mockPakovanje;
        private Mock<IVinskiPodrumiRepozitorijum> mockPodrumiRepo;
        private ProdajaServisAutomatski servis;

        [SetUp]
        public void Setup()
        {
            mockVinaRepo = new Mock<IVinaRepozitorijum>();
            mockLogger = new Mock<ILoggerServis>();
            mockFaktureRepo = new Mock<IFaktureRepozitorijum>();
            mockPaleteRepo = new Mock<IPaleteRepozitorijum>();
            mockSkladistenje = new Mock<ISkladistenjeServis>();
            mockProizvodnja = new Mock<IProizvodnjaVinaServis>();
            mockPakovanje = new Mock<IPakovanjeServis>();
            mockPodrumiRepo = new Mock<IVinskiPodrumiRepozitorijum>();

            servis = new ProdajaServisAutomatski(
                mockVinaRepo.Object,
                mockLogger.Object,
                mockFaktureRepo.Object,
                mockPaleteRepo.Object,
                mockSkladistenje.Object,
                mockProizvodnja.Object,
                mockPakovanje.Object,
                mockPodrumiRepo.Object
            );
        }

        [Test]
        public void DobijKatalog_VracaSvaVina_Uspesno()
        {
            var vina = new List<Vino>
            {
                new Vino("Vino 1", KategorijaVina.StolnoVino, 0.75, 1),
                new Vino("Vino 2", KategorijaVina.KvalitetnoVino, 1.5, 2)
            };

            mockVinaRepo.Setup(r => r.SvaVina()).Returns(vina);

            var rezultat = servis.DobijKatalog();

            Assert.That(rezultat.Count, Is.EqualTo(2));
            mockVinaRepo.Verify(r => r.SvaVina(), Times.Once);
        }

        [Test]
        public void KreirajFakturu_AutomatskiProizvodiAkoNedostaje_Uspesno()
        {
            var vino = new Vino("Chianti", KategorijaVina.KvalitetnoVino, 0.75, 1) { Id = 100 };
            var paleta = new Paleta("Milano", 1) { Id = 1, Status = StatusPalete.Otpremljena };
            var podrum = new VinskiPodrum("Glavni", 12.5, 10) { Id = 1 };

            mockVinaRepo.Setup(r => r.PronadjiVinoPoId(100)).Returns(vino);
            mockVinaRepo.Setup(r => r.PronadjiVinaPoKategoriji(KategorijaVina.KvalitetnoVino))
                .Returns(new List<Vino> { vino });
            mockPaleteRepo.Setup(r => r.PronadjiPaletePoStatusu(StatusPalete.Otpremljena))
                .Returns(new List<Paleta> { paleta });
            mockSkladistenje.Setup(s => s.IsporuciPalete(It.IsAny<int>()))
                .Returns(new List<Paleta> { paleta });
            mockFaktureRepo.Setup(r => r.DodajFakturu(It.IsAny<Faktura>()))
                .Returns((Faktura f) => { f.Id = 1; return f; });
            mockVinaRepo.Setup(r => r.ObrisiVino(It.IsAny<long>())).Returns(true);
            mockPodrumiRepo.Setup(r => r.SviVinskiPodrumi()).Returns(new List<VinskiPodrum> { podrum });

            var stavke = new List<(long, int)> { (100, 50) };
            var rezultat = servis.KreirajFakturu(TipProdaje.RestoranskaProadaja, NacinPlacanja.Gotovina, stavke);

            Assert.That(rezultat.Id, Is.Not.EqualTo(0));
            mockProizvodnja.Verify(p => p.ZapocniFermentaciju(It.IsAny<string>(), It.IsAny<KategorijaVina>(), It.IsAny<int>(), It.IsAny<double>()), Times.Once);
        }

        [Test]
        public void KreirajFakturu_BriseProdataVina_Uspesno()
        {
            var vino = new Vino("Merlot", KategorijaVina.PremijumVino, 0.75, 1) { Id = 200 };
            var paleta = new Paleta("Roma", 1) { Id = 2, Status = StatusPalete.Otpremljena };
            var podrum = new VinskiPodrum("Glavni", 12.5, 10) { Id = 1 };

            mockVinaRepo.Setup(r => r.PronadjiVinoPoId(200)).Returns(vino);
            mockVinaRepo.Setup(r => r.PronadjiVinaPoKategoriji(KategorijaVina.PremijumVino))
                .Returns(new List<Vino> { vino, vino });
            mockPaleteRepo.Setup(r => r.PronadjiPaletePoStatusu(StatusPalete.Otpremljena))
                .Returns(new List<Paleta> { paleta });
            mockSkladistenje.Setup(s => s.IsporuciPalete(It.IsAny<int>()))
                .Returns(new List<Paleta> { paleta });
            mockFaktureRepo.Setup(r => r.DodajFakturu(It.IsAny<Faktura>()))
                .Returns((Faktura f) => { f.Id = 2; return f; });
            mockVinaRepo.Setup(r => r.ObrisiVino(It.IsAny<long>())).Returns(true);
            mockPodrumiRepo.Setup(r => r.SviVinskiPodrumi()).Returns(new List<VinskiPodrum> { podrum });

            var stavke = new List<(long, int)> { (200, 2) };
            var rezultat = servis.KreirajFakturu(TipProdaje.DiskontPica, NacinPlacanja.Predracun, stavke);

            Assert.That(rezultat.Id, Is.EqualTo(2));
            mockVinaRepo.Verify(r => r.ObrisiVino(It.IsAny<long>()), Times.Exactly(2));
        }

        [Test]
        public void KreirajFakturu_VracaPraznuFakturuAkoNemaPaleta_Uspesno()
        {
            var vino = new Vino("Test", KategorijaVina.StolnoVino, 0.75, 1) { Id = 300 };

            mockVinaRepo.Setup(r => r.PronadjiVinoPoId(300)).Returns(vino);
            mockVinaRepo.Setup(r => r.PronadjiVinaPoKategoriji(KategorijaVina.StolnoVino))
                .Returns(new List<Vino> { vino });
            mockPaleteRepo.Setup(r => r.PronadjiPaletePoStatusu(StatusPalete.Otpremljena))
                .Returns(new List<Paleta>());
            mockSkladistenje.Setup(s => s.IsporuciPalete(It.IsAny<int>()))
                .Returns(new List<Paleta>());

            var stavke = new List<(long, int)> { (300, 1) };
            var rezultat = servis.KreirajFakturu(TipProdaje.RestoranskaProadaja, NacinPlacanja.Gotovina, stavke);

            Assert.That(rezultat.Id, Is.EqualTo(0));
        }

        [Test]
        public void PregledFaktura_VracaSveFakture_Uspesno()
        {
            var fakture = new List<Faktura>
            {
                new Faktura(TipProdaje.RestoranskaProadaja, NacinPlacanja.Gotovina),
                new Faktura(TipProdaje.DiskontPica, NacinPlacanja.Predracun)
            };

            mockFaktureRepo.Setup(r => r.SveFakture()).Returns(fakture);

            var rezultat = servis.PregledFaktura();

            Assert.That(rezultat.Count, Is.EqualTo(2));
            mockFaktureRepo.Verify(r => r.SveFakture(), Times.Once);
        }
    }
}