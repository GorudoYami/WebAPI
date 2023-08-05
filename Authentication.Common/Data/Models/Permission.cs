using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Authentication.Data.Models;

public class Permission {
	[Key]
	[Required]
	public int PermissionID { get; set; }

	[Required]
	public string Name { get; set; }

	[Required]
	public string Description { get; set; }

	public ICollection<PermissionGrant> PermissionSets { get; set; }
	public ICollection<Role> Roles { get; set; }
}

