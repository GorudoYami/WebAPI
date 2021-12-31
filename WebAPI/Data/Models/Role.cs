using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Data.Models {
	[Index("Name", IsUnique = true)]
	public class Role {
		[Required]
		public int RoleID { get; set; }

		[Required]
		[Column(TypeName = "varchar(50)")]
		public string Name { get; set; }

		[Required]
		[Column(TypeName = "text")]
		public string Description { get; set; }

		public ICollection<Account> Accounts { get; set; }
		public ICollection<Permission> Permissions { get; set; }
		public ICollection<PermissionSet> PermissionSets { get; set; }
	}
}
