using Domain.Interfaci;
using Domain.Modeli;
using Domain.Modeli.Enumeracije;
using System.Runtime.CompilerServices;

namespace Presentation.Meni
{
    public class OpcijeMeni
    {
        private readonly Korisnik prijavljen;
        private readonly IVinogradarstvoServis vinogradarstvo;
        private readonly IProizvodnjaVinaServis proizvodnjaVinaServis;
        private readonly IPakovanjeServis pakovanjeServis;
        private readonly ISkladistenjeServis skladistenjeServis;
        private readonly IProdajaServis prodajaServis;
        private readonly IVinskiPodrumServis vinskiPodrumServis;
        private readonly ILoggerServis loggerServis;

        public OpcijeMeni(
            Korisnik prijavljen,
            IVinogradarstvoServis vinogradarstvo,
            IProizvodnjaVinaServis proizvodnjaVinaServis,
            IPakovanjeServis pakovanjeServis,
            ISkladistenjeServis skladistenjeServis,
            IProdajaServis prodajaServis,
            IVinskiPodrumServis vinskiPodrumServis,
            ILoggerServis loggerServis)
        {
            this.prijavljen = prijavljen;
            this.vinogradarstvo = vinogradarstvo;
            this.proizvodnjaVinaServis = proizvodnjaVinaServis;
            this.pakovanjeServis = pakovanjeServis;
            this.skladistenjeServis = skladistenjeServis;
            this.prodajaServis = prodajaServis;
            this.vinskiPodrumServis = vinskiPodrumServis;
            this.loggerServis = loggerServis;
        }


        public void PrikaziMeni()
        {
            Console.WriteLine("\n============================================ Meni ===========================================");

            bool kraj = false;
            while (!kraj)
            {
                Console.WriteLine("\n--- OpcijeMeni");
                Console.WriteLine("1. Vinogradarstvo");
                Console.WriteLine("2. Proizvodnja Vina");
                Console.WriteLine("3. Pakovanje");
                Console.WriteLine("4. Skladištenje");
                Console.WriteLine("5. Prodaja");


                if (prijavljen.Uloga == TipKorisnika.GlavniEnolog)
                {
                    Console.WriteLine("6. Pregled faktura");
                }

                Console.WriteLine("0. Izlaz");
                Console.WriteLine("\nIzaberite opciju");


                string? izbor = Console.ReadLine();


                switch (izbor)
                {

                    case "1":
                        VinogradarstvoMeni();
                        break;
                    case "2":
                        ProizvodnjaVinaMeni();
                        break;
                    case "3":
                        PakovanjeMeni();
                        break;
                    case "4":
                        SkladistenjeMeni();
                        break;
                    case "5":
                        ProdajaMeni();
                        break;
                    case "6":
                        if (prijavljen.Uloga == TipKorisnika.GlavniEnolog)
                            PregledFakturaMeni();
                        else
                            Console.WriteLine("Nemate pristup ovoj opciji.");
                        break;
                    case "0":
                        kraj = true;
                        Console.WriteLine("Dovidjenja!");
                        loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO, $"Korsnik {prijavljen.KorisnickoIme} se odjavio.");
                        break;
                    default:
                        Console.WriteLine("Nepoznata opcija probajte ponovo");
                        break;
                }
            }
        }

        private void VinogradarstvoMeni()
        {
            Console.WriteLine("\n--- VINOGRADARSTVO ---");
            Console.WriteLine("1. Posadi novu lozu");
            Console.WriteLine("2. Promeni nivo šećera");
            Console.WriteLine("3. Promeni Fazu Loze");
            Console.WriteLine("4. Oberi loze");
            Console.Write("Izaberite opciju: ");

            string? izbor = Console.ReadLine();

            switch (izbor)
            {
                case "1":
                    Console.Write("Naziv sorte: ");
                    string? naziv = Console.ReadLine();
                    Console.Write("Region uzgoja: ");
                    string? region = Console.ReadLine();

                    if (!string.IsNullOrEmpty(naziv) && !string.IsNullOrEmpty(region))
                    {
                        var loza = vinogradarstvo.PosadiNovuLozu(naziv, region);
                        if (loza.Id != 0)
                        {
                            Console.WriteLine($"Uspešno posađena loza ID {loza.Id}, nivo šećera: {loza.NivoSecera} Brix");
                        }
                    }
                    break;

                case "2":
                    Console.WriteLine("\n--- DOSTUPNE LOZE ---");
                    var lozeZaSecer = vinogradarstvo.DobijSveLoze();
                    foreach (var l in lozeZaSecer)
                    {
                        Console.WriteLine($"ID: {l.Id}, Naziv: {l.Naziv}, Faza: {l.FazaZrelosti}, Nivo šećera: {l.NivoSecera} Brix");
                    }

                    Console.Write("\nID loze: ");
                    if (long.TryParse(Console.ReadLine(), out long idLoze))
                    {
                        Console.Write("Procenat promene (+ ili -): ");
                        if (double.TryParse(Console.ReadLine(), out double procenat))
                        {
                            var loza = vinogradarstvo.PromeniNivoSecera(idLoze, procenat);

                            if (loza.Id == 0)
                            {
                                Console.WriteLine("Greska pri promeni nivoa šećera.");
                            }
                            else
                            {
                                Console.WriteLine($"Nivo šećera je promenjen na {loza.NivoSecera} Brix.");
                            }
                        }
                    }
                    break;

                case "3":
                    Console.WriteLine("\n--- DOSTUPNE LOZE ---");
                    var lozeZaFazu = vinogradarstvo.DobijSveLoze();
                    foreach (var l in lozeZaFazu)
                    {
                        Console.WriteLine($"ID: {l.Id}, Naziv: {l.Naziv}, Faza: {l.FazaZrelosti}");
                    }

                    Console.Write("\nID loze: ");
                    if (long.TryParse(Console.ReadLine(), out long idLozeStanje))
                    {
                        Console.WriteLine("\nFaze:");
                        Console.WriteLine("1. Posađena");
                        Console.WriteLine("2. Cveta");
                        Console.WriteLine("3. Zrenje");
                        Console.WriteLine("4. SpremnaZaBerbu");
                        Console.WriteLine("5. Obrana");
                        Console.Write("Nova faza: ");

                        if (int.TryParse(Console.ReadLine(), out int faza) && faza >= 1 && faza <= 5)
                        {
                            var loza = vinogradarstvo.PromeniFazuZrelosti(idLozeStanje, (FazaZrelosti)(faza - 1));

                            if (loza.Id != 0)
                            {
                                Console.WriteLine($"Faza promenjena u: {loza.FazaZrelosti}");
                            }
                            else
                            {
                                Console.WriteLine("Greška pri promeni faze");
                            }
                        }
                    }
                    break;

                case "4":
                    Console.Write("Naziv sorte: ");
                    string? sorta = Console.ReadLine();
                    Console.Write("Broj loza: ");
                    if (int.TryParse(Console.ReadLine(), out int broj) && !string.IsNullOrEmpty(sorta))
                    {
                        var loze = vinogradarstvo.OberiLoze(sorta, broj);
                        if (loze.Count > 0)
                        {
                            Console.WriteLine($"Uspešno obrano {loze.Count} loza.");
                        }
                        else
                        {
                            Console.WriteLine("Nije moguće obrati loze.");
                        }
                    }
                    break;
            }
        }

        private void PakovanjeMeni()
        {
            Console.WriteLine("\n--- PAKOVANJE ---");
            Console.WriteLine("1. Pakuj Vino");
            Console.WriteLine("2. Pošalji paletu u podrum");
            Console.Write("Izaberite opciju: ");

            string? izbor = Console.ReadLine();

            switch (izbor)
            {
                case "1":
                    var podrumi = vinskiPodrumServis.DobijSvePodrume();

                    Console.WriteLine("\n--- DOSTUPNI VINSKI PODRUMI ---");
                    foreach (var p in podrumi)
                    {
                        Console.WriteLine($"ID: {p.Id}, Naziv: {p.Naziv}, Temp: {p.TemperaturaSkladistenja}°C, Max paleta: {p.MaksimalanBrojPaleta}");
                    }

                    Console.Write($"\nID vinskog podruma: ");
                    if (long.TryParse(Console.ReadLine(), out long idPodruma))
                    {
                        Console.Write("Adresa odredišta: ");
                        string? adresa = Console.ReadLine();

                        var katalog = prodajaServis.DobijKatalog();

                        Console.WriteLine("\n--- DOSTUPNA VINA ---");
                        foreach (var v in katalog)
                        {
                            Console.WriteLine($"ID: {v.Id}, Naziv: {v.Naziv}, Kategorija: {v.Kategorija}, Zapremina: {v.Zapremina}L");
                        }

                        Console.Write("\nID-evi vina (odvojeni zarezom): ");
                        string? idVina = Console.ReadLine();

                        if (!string.IsNullOrEmpty(adresa) && !string.IsNullOrEmpty(idVina))
                        {
                            List<long> IdList = idVina.Split(',').Select(x => long.Parse(x.Trim())).ToList();
                            var paleta = pakovanjeServis.PakujVino(idPodruma, adresa, IdList);
                            if (paleta.Id != 0)
                            {
                                Console.WriteLine($"Paleta {paleta.Sifra} uspešno kreirana.");
                            }
                        }
                    }
                    break;

                case "2":
                    Console.WriteLine("\n--- DOSTUPNE PALETE ---");
                    var palete = pakovanjeServis.DobijSvePalete();
                    foreach (var p in palete)
                    {
                        Console.WriteLine($"ID: {p.Id}, Šifra: {p.Sifra}, Status: {p.Status}, Broj vina: {p.IdVina.Count}");
                    }

                    Console.Write("\nID palete: ");
                    if (long.TryParse(Console.ReadLine(), out long idPalete))
                    {
                        bool uspeh = pakovanjeServis.PosaljiPaletuUPodrum(idPalete);
                        Console.WriteLine(uspeh ? "Paleta uspešno poslata." : "Greška pri slanju palete.");
                    }
                    break;
            }
        }

        private void SkladistenjeMeni()
        {
            Console.WriteLine("\n--- SKLADIŠTENJE  ---");
            Console.WriteLine("Unesite broj paleta koji želite da se skladištite: ");

            if (int.TryParse(Console.ReadLine(), out int broj))
            {
                var palete = skladistenjeServis.IsporuciPalete(broj);
                Console.WriteLine($"Skladišteno je {palete.Count} paleta za isporuku!");
            }
        }

        private void PregledFakturaMeni()
        {
            Console.WriteLine("\n--- PREGLED FAKTURA ---");
            var fakture = prodajaServis.PregledFaktura();

            Console.WriteLine($"\nUkupno faktura: {fakture.Count}\n");

            foreach (var faktura in fakture)
            {
                Console.WriteLine($"ID: {faktura.Id}");
                Console.WriteLine($"Datum: {faktura.DatumKreiranja}");
                Console.WriteLine($"Tip prodaje: {faktura.TipProdaje}");
                Console.WriteLine($"Način plaćanja: {faktura.NacinPlacanja}");
                Console.WriteLine($"Stavki: {faktura.Stavke.Count}");
                Console.WriteLine($"Ukupan iznos: {faktura.UkupanIznos:F2} EUR");
                Console.WriteLine("---");
            }
        }


        public void ProizvodnjaVinaMeni()
        {
            Console.WriteLine("\n---PROIZVODNJA VINA ---");
            Console.WriteLine("Odaberi akciju:");
            Console.WriteLine("1. Zapocni fermentaciju");
            Console.WriteLine("2. Dobij proizvedena vina");

            string? izbor = Console.ReadLine();

            switch (izbor)
            {
                case "1":
                    Console.WriteLine($"\nNaziv vina:");
                    string? naziv = Console.ReadLine();
                    Console.WriteLine("Kategorija (0-Stolno, 1-Kvalitetno, 2-Premium): ");
                    if (int.TryParse(Console.ReadLine(), out int kat))
                    {
                        Console.Write("Broj flaša: ");
                        if (int.TryParse(Console.ReadLine(), out int brojFlasa))
                        {
                            Console.WriteLine("Zapremina flaše (0.75 ili 1.5): ");
                            if (double.TryParse(Console.ReadLine(), out double zapremina) && !string.IsNullOrEmpty(naziv))
                            {
                                var vina = proizvodnjaVinaServis.ZapocniFermentaciju(naziv, (KategorijaVina)kat, brojFlasa, zapremina);
                                Console.WriteLine($"Fermentacija završena. Proizvedeno {vina.Count} flaša");
                            }
                        }
                    }
                    break;
                case "2":
                    Console.WriteLine("Kategorija (0-Stolno, 1-Kvalitetno, 2-Premium): ");
                    if (int.TryParse(Console.ReadLine(), out int kategorija))
                    {
                        Console.Write("Količina: ");
                        if (int.TryParse(Console.ReadLine(), out int kolicina))
                        {
                            var vina = proizvodnjaVinaServis.DobijProizvedenaVina((KategorijaVina)kategorija, kolicina);
                            Console.WriteLine($"Dobijeno {vina.Count} vina.");
                        }
                    }
                    break;
                default:
                    Console.WriteLine("\nPogresan izbor !");
                    break;
            }
        }


        private void ProdajaMeni()
        {
            Console.WriteLine("--- PRODAJA VINA ---");
            Console.WriteLine("1. Katalog");
            Console.WriteLine("2. Kreiraj Fakturu");
            Console.WriteLine("\nIzaberite opciju");
            string? izbor = Console.ReadLine();
            switch (izbor)
            {
                case "1":
                    Console.WriteLine("\n--- KATALOG ---\n");
                    List<Vino> katalog = prodajaServis.DobijKatalog();
                    foreach (Vino k in katalog)
                    {
                        Console.WriteLine($"ID: {k.Id}, Naziv: {k.Naziv}, Kategorija: {k.Kategorija}, Zapremina: {k.Zapremina}L\n");
                    }
                    break;
                case "2":
                    Console.WriteLine("\n--- PRODAJA ---");
                    Console.WriteLine("Tip prodaje (0-Restoranska, 1-Diskont): ");
                    if (int.TryParse(Console.ReadLine(), out int tip))
                    {
                        Console.WriteLine("Način plaćanja (0-Gotovina, 1-Predračun, 2-Gotovinski račun): ");
                        if (int.TryParse(Console.ReadLine(), out int nacin))
                        {
                            Console.WriteLine("\n--- DOSTUPNA VINA ---");
                            var katalogZaProdaju = prodajaServis.DobijKatalog();
                            foreach (var v in katalogZaProdaju)
                            {
                                Console.WriteLine($"ID: {v.Id}, Naziv: {v.Naziv}, Kategorija: {v.Kategorija}, Zapremina: {v.Zapremina}L");
                            }

                            List<(long, int)> stavke = new List<(long, int)>();
                            bool dodavanjeStavki = true;

                            while (dodavanjeStavki)
                            {
                                Console.Write("\nID vina (0 za kraj): ");
                                if (long.TryParse(Console.ReadLine(), out long idVina))
                                {
                                    if (idVina == 0)
                                    {
                                        dodavanjeStavki = false;
                                        continue;
                                    }

                                    Console.Write("Količina: ");
                                    if (int.TryParse(Console.ReadLine(), out int kolicina))
                                    {
                                        stavke.Add((idVina, kolicina));
                                    }
                                }
                            }

                            if (stavke.Count > 0)
                            {
                                var faktura = prodajaServis.KreirajFakturu((TipProdaje)tip, (NacinPlacanja)nacin, stavke);
                                if (faktura.Id != 0)
                                {
                                    Console.WriteLine($"\nFaktura kreirana! ID: {faktura.Id}");
                                    Console.WriteLine($"Ukupan iznos: {faktura.UkupanIznos:F2} EUR");
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}