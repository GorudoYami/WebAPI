using System.ComponentModel.DataAnnotations;

namespace WebAPI.Data.Models;

public class GlobalVariable {
	[Required]
	[Key]
	public string Name { get; set; }
	[Required]
	public string Value { get; set; }
}