using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Data.Models {
	[Index("Username", IsUnique = true)]
	[Index("Email", IsUnique = true)]
	public class Account {
		[Key]
		[Required]
		public int AccountID { get; set; }

		[Required]
		[Column(TypeName = "varchar(100)")]
		public string Email { get; set; }

		[Required]
		[Column(TypeName = "varchar(50)")]
		public string Username { get; set; }

		[Required]
		[Column(TypeName = "varchar(88)")]
		public string Password { get; set; }

		[Required]
		[Column(TypeName = "varchar(24)")]
		public string Salt { get; set; }

		[Required]
		public int RoleID { get; set; }

		[Required]
		public Role Role { get; set; }
	}
}
