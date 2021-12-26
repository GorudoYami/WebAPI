using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data.Models {
	public class PermissionSet {
		[Required]
		[ForeignKey("FK_Roles")]
		public int RoleID { get; set; }

		[Required]
		[ForeignKey("FK_Permissions")]
		public int PermissionID { get; set; }

		[Required]
		public bool Value { get; set; }
	}
}
