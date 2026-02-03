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

namespace Tests
{
    [TestFixture]
    public class ProdajaServisManuelniTests
    {
        private Mock<IVinaRepozitorijum> mockVinaRepo;
        private Mock<ILoggerServis> mockLogger;
        private Mock<IFaktureRepozitorijum> mockFaktureRepo;
        private Mock<ISkladistenjeServis> mockSkladistenje;
        private Mock<IResursKalkulatorServis> mockResursKalkulator;
        private List<ICenovnaStrategija> cenovneStrategije;
        private List<IPopustStrategija> popustStrategije;
        private ProdajaServisManuelni servis;

        [SetUp]
        public void Setup()
        {
            mockVinaRepo = new Mock<IVinaRepozitorijum>();
            mockLogger = new Mock<ILoggerServis>();
            mockFaktureRepo = new Mock<IFaktureRepozitorijum>();
            mockSkladistenje = new Mock<ISkladistenjeServis>();
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

            servis = new ProdajaServisManuelni(
                mockVinaRepo.Object,
                mockLogger.Object,
                mockFaktureRepo.Object,
                mockSkladistenje.Object,
                mockResursKalkulator.Object,
                cenovneStrategije,
                popustStrategije
            );
        }

        [Test]
        public void KreirajFakturu_ManuelniMod_KoristiCenovneStrategije()
        {
            var vino = new Vino("Chianti", KategorijaVina.KvalitetnoVino, 0.75, 1) { Id = 100 };
            var paleta = new Paleta("Milano", 1) { Id = 1, Status = StatusPalete.Otpremljena };

            mockVinaRepo.Setup(r => r.PronadjiVinoPoId(100)).Returns(vino);
            mockResursKalkulator.Setup(r => r.IzracunajPotrebanBrojPaleta(10)).Returns(1);
            mockSkladistenje.Setup(s => s.IsporuciPalete(It.IsAny<int>()))
                .Returns(new List<Paleta> { paleta });
            mockFaktureRepo.Setup(r => r.DodajFakturu(It.IsAny<Faktura>()))
                .Returns((Faktura f) => { f.Id = 1; return f; });

            var stavke = new List<(long, int)> { (100, 10) };
            var rezultat = servis.KreirajFakturu(TipProdaje.RestoranskaProadaja, NacinPlacanja.Gotovina, stavke);

            Assert.That(rezultat.Id, Is.Not.EqualTo(0));
            Assert.That(rezultat.Stavke[0].CenaPoJedinici, Is.EqualTo(11.25));
        }

        [Test]
        public void KreirajFakturu_VracaPraznuFakturuBezPaleta_Uspesno()
        {
            var vino = new Vino("Test", KategorijaVina.StolnoVino, 0.75, 1) { Id = 200 };

            mockVinaRepo.Setup(r => r.PronadjiVinoPoId(200)).Returns(vino);
            mockResursKalkulator.Setup(r => r.IzracunajPotrebanBrojPaleta(5)).Returns(1);
            mockSkladistenje.Setup(s => s.IsporuciPalete(It.IsAny<int>()))
                .Returns(new List<Paleta>());

            var stavke = new List<(long, int)> { (200, 5) };
            var rezultat = servis.KreirajFakturu(TipProdaje.DiskontPica, NacinPlacanja.Predracun, stavke);

            Assert.That(rezultat.Id, Is.EqualTo(0));
        }
    }
}