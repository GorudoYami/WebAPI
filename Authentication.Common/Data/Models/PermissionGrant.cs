using System.ComponentModel.DataAnnotations;

namespace Authentication.Data.Models;

public class PermissionGrant {

	[Required]
	public int PermissionId { get; set; }
	[Required]
	public Permission Permission { get; set; }

	[Required]
	public int RoleId { get; set; }
	[Required]
	public Role Role { get; set; }
}
