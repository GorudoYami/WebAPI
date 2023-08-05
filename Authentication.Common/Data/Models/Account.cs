using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Data.Models;

public class Account {
	[Key]
	[Required]
	public int Id { get; set; }

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
	public int RoleId { get; set; }

	[Required]
	public Role Role { get; set; }
}
