﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace WebAPI.Data.TransferObjects {
	public class LoginDTO {
		public string Login { get; set; }
		public string Password { get; set; }
	}
}
