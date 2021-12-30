using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Data.Models {
	public class Account {
		[Required]
		public int AccountID { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string Salt { get; set; }

		[Required]
		public int RoleID { get; set; }

		[Required]
		public Role Role { get; set; }

		public ICollection<Permission> PermissionGrants {
			get => Role.PermissionGrants;
			set => Role.PermissionGrants = value;
		}

		public ICollection<Permission> PermissionDenies {
			get => Role.PermissionDenies;
			set => Role.PermissionDenies = value;
		}
	}
}
