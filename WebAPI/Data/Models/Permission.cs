using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
