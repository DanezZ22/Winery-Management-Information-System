# Informacioni Sistem za Upravljanje Vinarijom

## Autori
- **Tim 08** - ERS Projekat 2025-2026

## Pregled Projekta

Informacioni sistem za upravljanje vinarijom u Toskani razvijen u programskom jeziku C# sa XML bazom podataka. Sistem implementira Clean Architecture principe i podržava kompletan proces od vinogradarstva do prodaje vina.

## Tehnologije

- **Programski jezik:** C# (.NET 8.0)
- **Arhitektura:** Clean Architecture, Domain-Driven Design
- **Baza podataka:** XML (XmlSerializer)
- **Testiranje:** NUnit 3.x, Moq 4.x
- **Dependency Injection:** Constructor Injection Pattern
- **Design Patterns:** Repository Pattern, Strategy Pattern, Observer Pattern

## Funkcionalnosti

### Vinogradarstvo
- Sađenje novih loza sa automatskim generisanjem nivoa šećera (15-28 Brix)
- Praćenje faza zrelosti loza (Posađena, Cveta, Zrenje, Spremna za berbu, Obrana)
- Promena nivoa šečera u lozama
- Berba loza spremnih za proizvodnju

### Proizvodnja Vina
- Automatska fermentacija sa kalkulacijom potrebnih resursa
- Automatsko sađenje dodatnih loza ukoliko nedostaju
- Balansiranje nivoa šećera za optimalan kvalitet vina
- Podržane kategorije: Stolno vino, Kvalitetno vino, Premium vino
- Podržane zapremine: 0.75L i 1.5L

### Pakovanje i Skladištenje
- Pakovanje vina u palete (24 flaše po paleti)
- Slanje paleta u vinske podrume
- Dve strategije skladištenja:
  - Vinski podrum: 0.3s po paleti
  - Lokalni kelar: 1.8s po paleti
- Praćenje statusa paleta (Upakovana, Otpremljena)

### Prodaja
- Dva moda rada:
  - **Automatski mod (Kupac):** Potpuna automatizacija proizvodnje, pakovanja i skladištenja
  - **Manuelni mod (Glavni Enolog / Kelar Majstor):** Ručna kontrola svih procesa
- Kreiranje faktura sa automatskim kalkulisanjem cena
- Tipovi prodaje: Restoranska prodaja, Diskont pića
- Načini plaćanja: Gotovina, Predračun, Gotovinski račun
- Automatsko brisanje prodatih vina iz kataloga

### Autentifikacija
- Uloge korisnika:
  - **Glavni Enolog:** Potpun pristup, uključujući pregled faktura
  - **Kelar Majstor:** Pristup svim operacijama osim pregleda faktura
  - **Kupac:** Pristup samo katalogu i prodaji (automatski mod)

### Logovanje
- Evidencija svih kritičnih operacija u `vinarija.log`
- Nivoi evidencije: INFO, WARNING, ERROR
- Timestamp za svaki događaj

## Instalacija i Pokretanje

### Preduslov
- .NET 8.0 SDK ili noviji
- Git

### Kloniranje repozitorijuma
```bash
git clone https://github.com/ERS-Projekat/ers_-02_tim08.git
cd ers_-02_tim08
```

### Build aplikacije
```bash
dotnet build
```

### Pokretanje aplikacije
```bash
cd Loger_Bloger
dotnet run
```

### Pokretanje testova
```bash
cd Tests
dotnet test
```

Za detaljniji output testova:
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Inicijalni Podaci

Sistem automatski kreira sledeće korisničke naloge pri prvom pokretanju:

| Korisničko ime | Lozinka | Uloga | Privilegije |
|----------------|---------|-------|-------------|
| enolog | enolog123 | Glavni Enolog | Sve funkcionalnosti + pregled faktura |
| kelar | kelar123 | Kelar Majstor | Sve funkcionalnosti osim pregleda faktura |
| kupac | kupac123 | Kupac | Katalog + prodaja (automatski mod) |

Početna baza takođe sadrži:
- 5 inicijalizovanih loza različitih sorti
- 2 vinska podruma
- 5 primera vina različitih kategorija i zapremina

## Testiranje

Projekat sadrži sveobuhvatnu test suite sa 28 unit testova:

### Testovi Modela (10 testova)
- **LozaTests:** Validacija konstruktora, nivoa šećera, faza zrelosti
- **VinoTests:** Validacija konstruktora, šifre serije, zapremina
- **PaletaTests:** Validacija konstruktora, dodavanja vina, statusa
- **FakturaTests:** Validacija datuma kreiranja, automatskog kalkulisanja iznosa

### Testovi Servisa (18 testova)
- **VinogradarstvoServisTests:** Testovi sađenja, promene nivoa šećera, berbe
- **ProizvodnjaVinaServisTests:** Testovi fermentacije, automatskog sađenja, dobavljanja vina
- **ProdajaServisAutomatskiTests:** Testovi automatske proizvodnje, brisanja vina, kreiranja faktura
- **ProdajaServisManuelniTests:** Testovi manuelnog moda bez automatizacije
- **AutentifikacioniServisTests:** Testovi login funkcionalnosti za sve uloge

Svi testovi koriste:
- **NUnit 3.x** framework sa Assert.That sintaksom
- **Moq** biblioteku za mockovanje zavisnosti
- **Setup/TearDown** pattern za pripremu test okruženja

## Arhitektonski Principi

### Clean Architecture
Projekat striktno poštuje principe Clean Architecture sa jasnom separacijom slojeva:
- Domain sloj ne zavisi ni od čega
- Application sloj zavisi samo od Domain sloja
- Infrastructure sloj implementira interfejse iz Domain sloja
- Presentation sloj zavisi od Application i Domain sloja

### SOLID Principi
- **Single Responsibility:** Svaki servis ima jasno definisanu odgovornost
- **Open/Closed:** Proširivost kroz interfejse i nasledjivanje
- **Liskov Substitution:** Polimorfizam kroz ISkladistenjeServis interfejs
- **Interface Segregation:** Fokusirani interfejsi bez nepotrebnih metoda
- **Dependency Inversion:** Zavisnost od apstrakcija, ne konkretnih implementacija

### Design Patterns
- **Repository Pattern:** Apstrakcija pristupa podacima
- **Strategy Pattern:** ProdajaServisAutomatski vs ProdajaServisManuelni
- **Dependency Injection:** Constructor injection kroz sve slojeve
- **Observer Pattern:** Logger servis za evidenciju događaja

## Biznis Logika

### Proizvodnja Vina - Kalkulacija

```
Potrebna količina vina = Broj flaša × Zapremina flaše
Potreban broj loza = Potrebna količina vina / 1.2L (prinos po lozi)
```

Primer:
- 100 flaša × 0.75L = 75L vina
- 75L / 1.2L = 63 loze potrebno

### Kalkulacija Cena

Bazne cene po kategoriji i zapremini:
- Stolno vino: 8 EUR/L
- Kvalitetno vino: 15 EUR/L
- Premium vino: 35 EUR/L

Popust za diskont pića: 15% (cena × 0.85)

Primer:
- Chianti Classico (Kvalitetno, 0.75L) = 0.75 × 15 = 11.25 EUR
- Sa diskontom = 11.25 × 0.85 = 9.56 EUR

### Pakovanje Paleta

- Maksimum 24 flaše po paleti
- Potreban broj paleta = ⌈Ukupno flaša / 24⌉

Primer:
- 100 flaša → ⌈100/24⌉ = 5 paleta

## Automatizacija vs Manuelni Mod

### Automatski Mod (Kupac)
Pri kreiranju fakture sistem automatski:
1. Grupise narudžbine po kategorijama vina
2. Proverava dostupnost vina u bazi
3. Automatski proizvodi nedostajuća vina (sađenje → berba → fermentacija)
4. Automatski pakuje vino u potreban broj paleta
5. Automatski šalje palete u vinski podrum
6. Poziva skladištenje za isporuku
7. Briše prodata vina iz kataloga
8. Kreira i čuva fakturu

### Manuelni Mod (Glavni Enolog / Kelar Majstor)
Korisnik mora ručno:
1. Proizvodnja vina → Zapocni fermentaciju
2. Pakovanje → Pakuj vino u palete
3. Pakovanje → Pošalji palete u podrum
4. Prodaja → Kreiraj fakturu

Sistem proverava da li postoje palete pre kreiranja fakture i prikazuje detaljne upute ako nedostaju resursi.

## Baza Podataka

Sistem koristi XML fajl (`vinarija.xml`) za perzistenciju podataka sa sledećim tabelama:
- Korisnici
- Loze
- Vina
- Palete
- Vinski podrumi
- Fakture

ID generisanje: `DateTimeOffset.Now.ToUnixTimeSeconds()` za jedinstvene identifikatore.

## Logovanje

Sve operacije se evidentiraju u `vinarija.log` fajl sa formatom:
```
[2025-01-14 15:30:45] [INFO] Kreirana faktura ID 1768303018, iznos: 956.00 EUR
[2025-01-14 15:30:45] [INFO] Prodato i obrisano 100 vina
```

## Poznati Problemi i Ograničenja

- XML baza nije optimizovana za velike količine podataka (500+ zapisa)
- Nema podrške za konekciju sa relacionim bazama podataka
- Autentifikacija koristi plain text lozinke (nije production-ready)
- Sistem ne podržava konkurentni pristup (multi-user environment)



## Kontakt

Za pitanja i sugestije kontaktirajte Tim 08 preko GitHub repozitorijuma.
