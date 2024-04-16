namespace TraderPerformance.ViewModels;

public class HomeViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal GainLoss { get; set; }
    public string GainLossColor { get; set; }
    public decimal ProgressWidth { get; set; }
}