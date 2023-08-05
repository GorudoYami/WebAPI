using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Data.Models;

public class GlobalVariable {
	[Key]
	[Required]
	public int Id { get; set; }

	[Required]
	[Column(TypeName = "varchar(50)")]
	public string Name { get; set; }

	[Required]
	[Column(TypeName = "varchar(100)")]
	public string Value { get; set; }

	[Required]
	[Column(TypeName = "text")]
	public string Description { get; set; }

	public T GetValue<T>() => (T)Convert.ChangeType(Value, typeof(T));
}