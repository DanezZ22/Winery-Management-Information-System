using NUnit.Framework;
using Moq;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using Services.ProdajaServisi;
using Services.CenovneStrategije;
using Services.PopustStrategije;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class ProdajaServisAutomatskiTests
    {
        private Mock<IVinaRepozitorijum> mockVinaRepo;
        private Mock<ILoggerServis> mockLogger;
        private Mock<IFaktureRepozitorijum> mockFaktureRepo;
        private Mock<ISkladistenjeServis> mockSkladistenje;
        private Mock<IAutomatskaProizvodnjaFacade> mockAutomatskaProizvodnja;
        private Mock<IResursKalkulatorServis> mockResursKalkulator;
        private List<ICenovnaStrategija> cenovneStrategije;
        private List<IPopustStrategija> popustStrategije;
        private ProdajaServisAutomatski servis;

        [SetUp]
        public void Setup()
        {
            mockVinaRepo = new Mock<IVinaRepozitorijum>();
            mockLogger = new Mock<ILoggerServis>();
            mockFaktureRepo = new Mock<IFaktureRepozitorijum>();
            mockSkladistenje = new Mock<ISkladistenjeServis>();
            mockAutomatskaProizvodnja = new Mock<IAutomatskaProizvodnjaFacade>();
            mockResursKalkulator = new Mock<IResursKalkulatorServis>();

            cenovneStrategije = new List<ICenovnaStrategija>
            {
                new StolnoVinoCenovnaStrategija(),
                new KvalitetnoVinoCenovnaStrategija(),
                new PremijumVinoCenovnaStrategija()
            };

            popustStrategije = new List<IPopustStrategija>
            {
                new BezPopustaStrategija(),
                new DiskontPopustStrategija()
            };

            servis = new ProdajaServisAutomatski(
                mockVinaRepo.Object,
                mockLogger.Object,
                mockFaktureRepo.Object,
                mockSkladistenje.Object,
                mockAutomatskaProizvodnja.Object,
                mockResursKalkulator.Object,
                cenovneStrategije,
                popustStrategije
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
        public void KreirajFakturu_KoristiCenovneStrategije_Uspesno()
        {
            var vino = new Vino("Chianti", KategorijaVina.KvalitetnoVino, 0.75, 1) { Id = 100 };
            var paleta = new Paleta("Milano", 1) { Id = 1, Status = StatusPalete.Otpremljena };

            mockVinaRepo.Setup(r => r.PronadjiVinoPoId(100)).Returns(vino);
            mockResursKalkulator.Setup(r => r.IzracunajPotrebanBrojPaleta(10)).Returns(1);
            mockSkladistenje.Setup(s => s.IsporuciPalete(It.IsAny<int>()))
                .Returns(new List<Paleta> { paleta });
            mockFaktureRepo.Setup(r => r.DodajFakturu(It.IsAny<Faktura>()))
                .Returns((Faktura f) => { f.Id = 1; return f; });
            mockVinaRepo.Setup(r => r.ObrisiVino(It.IsAny<long>())).Returns(true);

            var stavke = new List<(long, int)> { (100, 10) };
            var rezultat = servis.KreirajFakturu(TipProdaje.RestoranskaProadaja, NacinPlacanja.Gotovina, stavke);

            Assert.That(rezultat.Id, Is.Not.EqualTo(0));
            Assert.That(rezultat.Stavke[0].CenaPoJedinici, Is.EqualTo(11.25));
        }

        [Test]
        public void KreirajFakturu_PrimenjujePopust_Uspesno()
        {
            var vino = new Vino("Merlot", KategorijaVina.PremijumVino, 0.75, 1) { Id = 200 };
            var paleta = new Paleta("Roma", 1) { Id = 2, Status = StatusPalete.Otpremljena };

            mockVinaRepo.Setup(r => r.PronadjiVinoPoId(200)).Returns(vino);
            mockResursKalkulator.Setup(r => r.IzracunajPotrebanBrojPaleta(5)).Returns(1);
            mockSkladistenje.Setup(s => s.IsporuciPalete(It.IsAny<int>()))
                .Returns(new List<Paleta> { paleta });
            mockFaktureRepo.Setup(r => r.DodajFakturu(It.IsAny<Faktura>()))
                .Returns((Faktura f) => { f.Id = 2; return f; });
            mockVinaRepo.Setup(r => r.ObrisiVino(It.IsAny<long>())).Returns(true);

            var stavke = new List<(long, int)> { (200, 5) };
            var rezultat = servis.KreirajFakturu(TipProdaje.DiskontPica, NacinPlacanja.Predracun, stavke);

            Assert.That(rezultat.Id, Is.EqualTo(2));
            Assert.That(rezultat.Stavke[0].CenaPoJedinici, Is.EqualTo(22.31));
        }

        [Test]
        public void KreirajFakturu_PozvaAutomatskuProizvodnju_Uspesno()
        {
            var vino = new Vino("Test", KategorijaVina.StolnoVino, 0.75, 1) { Id = 300 };
            var paleta = new Paleta("Test", 1) { Id = 3, Status = StatusPalete.Otpremljena };

            mockVinaRepo.Setup(r => r.PronadjiVinoPoId(300)).Returns(vino);
            mockResursKalkulator.Setup(r => r.IzracunajPotrebanBrojPaleta(50)).Returns(3);
            mockSkladistenje.Setup(s => s.IsporuciPalete(It.IsAny<int>()))
                .Returns(new List<Paleta> { paleta });
            mockFaktureRepo.Setup(r => r.DodajFakturu(It.IsAny<Faktura>()))
                .Returns((Faktura f) => { f.Id = 3; return f; });
            mockVinaRepo.Setup(r => r.ObrisiVino(It.IsAny<long>())).Returns(true);

            var stavke = new List<(long, int)> { (300, 50) };
            var rezultat = servis.KreirajFakturu(TipProdaje.RestoranskaProadaja, NacinPlacanja.Gotovina, stavke);

            mockAutomatskaProizvodnja.Verify(
                a => a.ObezediVina(It.IsAny<Dictionary<KategorijaVina, int>>(), It.IsAny<List<(long, int)>>()),
                Times.Once
            );
            mockAutomatskaProizvodnja.Verify(a => a.ObezediPalete(50), Times.Once);
        }

        [Test]
        public void KreirajFakturu_BriseProdataVina_Uspesno()
        {
            var vino = new Vino("Delete Test", KategorijaVina.KvalitetnoVino, 0.75, 1) { Id = 400 };
            var paleta = new Paleta("Test", 1) { Id = 4, Status = StatusPalete.Otpremljena };

            mockVinaRepo.Setup(r => r.PronadjiVinoPoId(400)).Returns(vino);
            mockVinaRepo.Setup(r => r.PronadjiVinaPoKategoriji(KategorijaVina.KvalitetnoVino))
                .Returns(new List<Vino> { vino, vino });
            mockResursKalkulator.Setup(r => r.IzracunajPotrebanBrojPaleta(2)).Returns(1);
            mockSkladistenje.Setup(s => s.IsporuciPalete(It.IsAny<int>()))
                .Returns(new List<Paleta> { paleta });
            mockFaktureRepo.Setup(r => r.DodajFakturu(It.IsAny<Faktura>()))
                .Returns((Faktura f) => { f.Id = 4; return f; });
            mockVinaRepo.Setup(r => r.ObrisiVino(It.IsAny<long>())).Returns(true);

            var stavke = new List<(long, int)> { (400, 2) };
            var rezultat = servis.KreirajFakturu(TipProdaje.RestoranskaProadaja, NacinPlacanja.Gotovina, stavke);

            mockVinaRepo.Verify(r => r.ObrisiVino(It.IsAny<long>()), Times.Exactly(2));
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
        }
    }
}