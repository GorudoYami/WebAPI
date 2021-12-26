using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Models;

namespace WebAPI.Data {
	public class Database : DbContext {
		public Database(DbContextOptions<Database> options) : base(options) {

		}

		public DbSet<Account> Accounts { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<Role> Roles { get; set; }
	}
}
