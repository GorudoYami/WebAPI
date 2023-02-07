using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data.Models {
	[Index("Name", IsUnique = true)]
	public class GlobalVariable {
		[Key]
		[Required]
		public int GlobalVariableID { get; set; }

		[Required]
		[Column(TypeName = "varchar(50)")]
		public string Name { get; set; }

		[Required]
		[Column(TypeName = "varchar(100)")]
		public string Value { get; set; }

		[Required]
		[Column(TypeName = "text")]
		public string Description { get; set; }
	}
}