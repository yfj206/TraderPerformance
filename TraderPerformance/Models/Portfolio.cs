using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TraderPerformance.Models;

public class Portfolio
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; }

    [Required]
    public string Name { get; set; }

    public virtual ICollection<Trade> Trades { get; set; }

    [NotMapped]
    public decimal GainLoss { get; set; }

    [NotMapped]
    public decimal ProgressWidth { get; set; }
}