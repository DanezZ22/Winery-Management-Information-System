using Domain.Modeli.Enumeracije;
using Domain.Modeli;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Domain

{
    [TestFixture]
    public class VinoTests
    {
        [Test]
        public void Vino_KonstruktorPostavljaVrednosti_Uspesno()
        {
            var vino = new Vino("Chianti Premium", KategorijaVina.PremijumVino, 0.75, 12345);

            Assert.That(vino.Naziv, Is.EqualTo("Chianti Premium"));
            Assert.That(vino.Kategorija, Is.EqualTo(KategorijaVina.PremijumVino));
            Assert.That(vino.Zapremina, Is.EqualTo(0.75));
        }

        [Test]
        public void Vino_SifraSerijeSePravilnoFormatira_Uspesno()
        {
            var vino = new Vino("Test Vino", KategorijaVina.StolnoVino, 1.5, 100);
            vino.Id = 54321;
            vino.SifraSerije = $"VN-{DateTime.Now.Year}-{vino.Id}";

            Assert.That(vino.SifraSerije, Does.StartWith($"VN-{DateTime.Now.Year}-"));
            Assert.That(vino.SifraSerije, Does.Contain("54321"));
        }

        [Test]
        public void Vino_ZapremineImaValidneVrednosti_Uspesno()
        {
            var vino1 = new Vino("Vino 1", KategorijaVina.KvalitetnoVino, 0.75, 1);
            var vino2 = new Vino("Vino 2", KategorijaVina.PremijumVino, 1.5, 2);

            Assert.That(vino1.Zapremina, Is.EqualTo(0.75).Within(0.01));
            Assert.That(vino2.Zapremina, Is.EqualTo(1.5).Within(0.01));
        }
    }
}

