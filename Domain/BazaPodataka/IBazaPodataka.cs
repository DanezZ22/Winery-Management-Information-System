namespace Domain.BazaPodataka
{
    public interface IBazaPodataka
    {
        TabeleBazaPodataka Tabele { get; set; }
        bool SacuvajPromene();
    }
}
