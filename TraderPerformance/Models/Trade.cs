using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TraderPerformance.Models;

public class Trade
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; }  // Buy or Sell

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public Guid SecurityID { get; set; }

    [ForeignKey("SecurityID")]
    public virtual Security Security { get; set; }

    public Guid? PortfolioID { get; set; }

    [ForeignKey("PortfolioID")]
    public virtual Portfolio Portfolio { get; set; }
}
