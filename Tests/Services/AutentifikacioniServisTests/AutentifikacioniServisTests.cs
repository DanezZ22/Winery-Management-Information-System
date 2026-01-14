using NUnit.Framework;
using Moq;
using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using Domain.Repozitorijumi;
using Services.AutenftikacioniServisi;

namespace Tests.Services.AutentifikacioniServisTests
{
    [TestFixture]
    public class AutentifikacioniServisTests
    {
        private Mock<IKorisniciRepozitorijum> mockKorisniciRepo;
        private Mock<ILoggerServis> mockLogger;
        private AutentifikacioniServis servis;

        [SetUp]
        public void Setup()
        {
            mockKorisniciRepo = new Mock<IKorisniciRepozitorijum>();
            mockLogger = new Mock<ILoggerServis>();
            servis = new AutentifikacioniServis(mockKorisniciRepo.Object, mockLogger.Object);
        }

        [Test]
        public void Prijava_TacniPodaci_VracaTrueISaPunenKorisnikom()
        {
            var korisnik = new Korisnik("enolog", "enolog123", "Marko Markovic", TipKorisnika.GlavniEnolog);
            mockKorisniciRepo.Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu("enolog")).Returns(korisnik);

            var (uspeh, prijavljen) = servis.Prijava("enolog", "enolog123");

            Assert.That(uspeh, Is.True);
            Assert.That(prijavljen.KorisnickoIme, Is.EqualTo("enolog"));
            Assert.That(prijavljen.Uloga, Is.EqualTo(TipKorisnika.GlavniEnolog));
            mockLogger.Verify(l => l.EvidentirajDogadjaj(TipEvidencije.INFO, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Prijava_PogresnaLozinka_VracaFalseIPrazanKorisnik()
        {
            var korisnik = new Korisnik("enolog", "enolog123", "Marko Markovic", TipKorisnika.GlavniEnolog);
            mockKorisniciRepo.Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu("enolog")).Returns(korisnik);

            var (uspeh, prijavljen) = servis.Prijava("enolog", "pogresna");

            Assert.That(uspeh, Is.False);
            Assert.That(prijavljen.Id, Is.EqualTo(0));
            mockLogger.Verify(l => l.EvidentirajDogadjaj(TipEvidencije.WARNING, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Prijava_NepostojeciKorisnik_VracaFalseIPrazanKorisnik()
        {
            mockKorisniciRepo.Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu("nepostojeci"))
                .Returns(new Korisnik());

            var (uspeh, prijavljen) = servis.Prijava("nepostojeci", "bilo_sta");

            Assert.That(uspeh, Is.False);
            Assert.That(prijavljen.KorisnickoIme, Is.EqualTo(string.Empty));
            mockLogger.Verify(l => l.EvidentirajDogadjaj(TipEvidencije.WARNING, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Prijava_Kupac_VracaTacnuUlogu()
        {
            var kupac = new Korisnik("kupac", "kupac123", "Petar Petrovic", TipKorisnika.Kupac);
            mockKorisniciRepo.Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu("kupac")).Returns(kupac);

            var (uspeh, prijavljen) = servis.Prijava("kupac", "kupac123");

            Assert.That(uspeh, Is.True);
            Assert.That(prijavljen.Uloga, Is.EqualTo(TipKorisnika.Kupac));
        }
    }
}