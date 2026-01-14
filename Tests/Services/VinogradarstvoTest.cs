using Domain.Interfaci;
using Domain.Modeli.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Moq;
using NUnit.Framework;
using Services.VinogradarstvoServisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services
{
    [TestFixture]
    public class VinogradarstvoServisTests
    {
        private Mock<ILozeRepozitorijum> mockLozeRepo;
        private Mock<ILoggerServis> mockLogger;
        private VinogradarstvoServis servis;

        [SetUp]
        public void Setup()
        {
            mockLozeRepo = new Mock<ILozeRepozitorijum>();
            mockLogger = new Mock<ILoggerServis>();
            servis = new VinogradarstvoServis(mockLozeRepo.Object, mockLogger.Object);
        }

        [Test]
        public void PosadiNovuLozu_GeneriseNivoSeceraUValidnomOpsegu_Uspesno()
        {
            mockLozeRepo.Setup(r => r.DodajLozu(It.IsAny<Loza>()))
                .Returns((Loza l) => { l.Id = 1; return l; });

            var loza = servis.PosadiNovuLozu("Sangiovese", "Chianti");

            Assert.That(loza.NivoSecera, Is.InRange(15.0, 28.0));
            Assert.That(loza.Naziv, Is.EqualTo("Sangiovese"));
            mockLozeRepo.Verify(r => r.DodajLozu(It.IsAny<Loza>()), Times.Once);
        }

        [Test]
        public void PromeniNivoSecera_PovecavaNivoZaDatiProcenat_Uspesno()
        {
            var loza = new Loza("Merlot", 20.0, 2024, "Bordeaux") { Id = 1 };
            mockLozeRepo.Setup(r => r.PronadjiLozuPoId(1)).Returns(loza);
            mockLozeRepo.Setup(r => r.AzurirajLozu(It.IsAny<Loza>())).Returns(true);

            var rezultat = servis.PromeniNivoSecera(1, 10.0);

            Assert.That(rezultat.NivoSecera, Is.EqualTo(22.0).Within(0.01));
            mockLozeRepo.Verify(r => r.AzurirajLozu(It.IsAny<Loza>()), Times.Once);
        }

        [Test]
        public void OberiLoze_VracaSamoSpremneLoze_Uspesno()
        {
            var loze = new List<Loza>
            {
                new Loza("Chianti", 23.0, 2024, "Toskana") { Id = 1, FazaZrelosti = FazaZrelosti.SpremnaZaBerbu },
                new Loza("Chianti", 22.0, 2024, "Toskana") { Id = 2, FazaZrelosti = FazaZrelosti.SpremnaZaBerbu },
                new Loza("Chianti", 21.0, 2024, "Toskana") { Id = 3, FazaZrelosti = FazaZrelosti.Zrenje }
            };

            mockLozeRepo.Setup(r => r.PronadjiLozePoNazivu("Chianti")).Returns(loze);
            mockLozeRepo.Setup(r => r.AzurirajLozu(It.IsAny<Loza>())).Returns(true);

            var rezultat = servis.OberiLoze("Chianti", 2);

            Assert.That(rezultat.Count, Is.EqualTo(2));
            Assert.That(rezultat.All(l => l.FazaZrelosti == FazaZrelosti.Obrana), Is.True);
        }
    }
}
