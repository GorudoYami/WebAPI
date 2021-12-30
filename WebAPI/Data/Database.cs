using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Models;

namespace WebAPI.Data {
	public class Database : DbContext {
		public Database(DbContextOptions<Database> options) : base(options) {

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<Account>()
				.HasKey(a => a.AccountID)
				.HasName("PK_Accounts");
			modelBuilder.Entity<Role>()
				.HasKey(r => r.RoleID)
				.HasName("PK_Roles");
			modelBuilder.Entity<Permission>()
				.HasKey(p => p.PermissionID)
				.HasName("PK_Permissions");
			modelBuilder.Entity<GlobalVariable>()
				.HasKey(gv => gv.Name)
				.HasName("PK_GlobalVariables");

			modelBuilder.Entity<Account>()
				.HasOne(a => a.Role)
				.WithMany(r => r.Accounts)
				.HasForeignKey(a => a.RoleID)
				.HasConstraintName("FK_Accounts_RoleID");

			modelBuilder.Entity<Permission>()
				.HasMany<Role>()
				.WithMany(r => r.PermissionGrants);

			modelBuilder.Entity<Permission>()
				.HasMany<Role>()
				.WithMany(r => r.PermissionDenies);
		}

		public DbSet<Account> Accounts { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<GlobalVariable> GlobalVariables { get; set; }
	}
}