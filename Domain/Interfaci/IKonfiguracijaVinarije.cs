namespace Domain.Interfaci
{
    public interface IKonfiguracijaVinarije
    {
        string DefaultniRegion { get; }
        string AutomatskaAdresaIsporuke { get; }
        double PrinosPoLozi { get; }
        int VinaPopaleti { get; }
        double OptimalniBrix { get; }
    }
}