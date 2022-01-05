using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace WebAPI.Data.TransferObjects {
	public class PasswordDTO {
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
	}
}
