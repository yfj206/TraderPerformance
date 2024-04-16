using System.ComponentModel.DataAnnotations;

namespace TraderPerformance.Models;

public class Security
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string TickerSymbol { get; set; }

    [Required]
    public string Name { get; set; }

    public virtual ICollection<Trade> Trades { get; set; }
}