using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Data.Models {
	public class Role {
		[Required]
		public int RoleID { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		public ICollection<Account> Accounts { get; set; }
		public ICollection<Permission> PermissionGrants { get; set; }
		public ICollection<Permission> PermissionDenies { get; set; }
	}
}
