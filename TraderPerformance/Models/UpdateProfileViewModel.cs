using System.ComponentModel.DataAnnotations;

namespace TraderPerformance.Models;

public class UpdateProfileViewModel
{
	[Required]
	[Display(Name = "First Name")]
	public string FirstName { get; set; }

	[Required]
	[Display(Name = "Last Name")]
	public string LastName { get; set; }

	[Phone]
	[Display(Name = "Phone Number")]
	public string PhoneNumber { get; set; }
}
