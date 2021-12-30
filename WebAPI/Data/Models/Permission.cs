using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Data.Models {
	public class Permission {
		[Required]
		[Key]
		public int PermissionID { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
