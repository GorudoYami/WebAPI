using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Data.Models {
	[Index("Name", IsUnique = true)]
	public class Permission {
		[Key]
		[Required]
		public int PermissionID { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Description { get; set; }

		public ICollection<PermissionSet> PermissionSets { get; set; }
		public ICollection<Role> Roles { get; set; }
	}
}
