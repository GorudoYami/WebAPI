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
		[ForeignKey("FK_Roles")]
		public int RoleID { get; set; }

	}
}
