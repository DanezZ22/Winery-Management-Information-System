using Domain.Interfaci;
using Domain.Modeli.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;

public class VinskiPodrumServis : IVinskiPodrumServis
{
    private readonly IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum;
    private readonly ILoggerServis loggerServis;

    public VinskiPodrumServis(
        IVinskiPodrumiRepozitorijum vinskiPodrumiRepozitorijum,
        ILoggerServis loggerServis)
    {
        this.vinskiPodrumiRepozitorijum = vinskiPodrumiRepozitorijum;
        this.loggerServis = loggerServis;
    }

    public List<VinskiPodrum> DobijSvePodrume()
    {
        try
        {
            var podrumi = vinskiPodrumiRepozitorijum.SviVinskiPodrumi().ToList();
            loggerServis.EvidentirajDogadjaj(TipEvidencije.INFO,
                $"Prikazano {podrumi.Count} vinskih podruma");
            return podrumi;
        }
        catch (Exception ex)
        {
            loggerServis.EvidentirajDogadjaj(TipEvidencije.ERROR,
                $"Greška pri dobijanju podruma: {ex.Message}");
            return new List<VinskiPodrum>();
        }
    }
}