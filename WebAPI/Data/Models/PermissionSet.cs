using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Data.Models;

[Index(new string[] { "PermissionID", "RoleID" }, IsUnique = true)]
public class PermissionSet {
	[Key, Required]
	public int PermissionSetID { get; set; }

	[Required]
	public int PermissionID { get; set; }
	[Required]
	public Permission Permission { get; set; }

	[Required]
	public int RoleID { get; set; }
	[Required]
	public Role Role { get; set; }

	[Required(ErrorMessage = "You must specify if the permission is granted or denied!")]
	public bool Value { get; set; }
}
