using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Data.Models;

public class Role {
	[Required]
	public int Id { get; set; }

	[Required]
	[Column(TypeName = "varchar(50)")]
	public string Name { get; set; }

	[Required]
	[Column(TypeName = "text")]
	public string Description { get; set; }

	public ICollection<Account> Accounts { get; set; }
	public ICollection<Permission> Permissions { get; set; }
	public ICollection<PermissionGrant> PermissionSets { get; set; }
}
