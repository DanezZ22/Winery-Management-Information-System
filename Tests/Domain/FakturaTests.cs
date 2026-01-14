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
    public class FakturaTests
    {
        [Test]
        public void Faktura_KonstruktorPostavljaDatum_Uspesno()
        {
            var faktura = new Faktura(TipProdaje.RestoranskaProadaja, NacinPlacanja.Gotovina);

            Assert.That(faktura.DatumKreiranja, Is.Not.EqualTo(new DateTime(1, 1, 1)));
            Assert.That(faktura.DatumKreiranja.Year, Is.EqualTo(DateTime.Now.Year));
        }

        [Test]
        public void Faktura_UkupanIznosSeAutomatskiracuna_Uspesno()
        {
            var faktura = new Faktura(TipProdaje.DiskontPica, NacinPlacanja.Predracun);
            faktura.Stavke.Add(new StavkaFakture(1, 10, 15.0));
            faktura.Stavke.Add(new StavkaFakture(2, 5, 20.0));

            Assert.That(faktura.UkupanIznos, Is.EqualTo(250.0));
        }

        [Test]
        public void Faktura_PrazanKonstruktorPostavljaDatum_Uspesno()
        {
            var faktura = new Faktura();

            Assert.That(faktura.DatumKreiranja, Is.Not.EqualTo(new DateTime(1, 1, 1)));
        }
    }
}