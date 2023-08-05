using Microsoft.EntityFrameworkCore;
using Authentication.Data.Models;

namespace Authentication.Data;

public class DatabaseContext : DbContext {
	public DatabaseContext() {

	}

	public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {

	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<Account>()
			.HasOne(a => a.Role)
			.WithMany(r => r.Accounts)
			.HasForeignKey(a => a.RoleId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Permission>()
			.HasMany(p => p.Roles)
			.WithMany(r => r.Permissions)
			.UsingEntity<PermissionGrant>
			(
				j => j
					.HasOne(ps => ps.Role)
					.WithMany(r => r.PermissionSets)
					.HasForeignKey(ps => ps.RoleId)
					.OnDelete(DeleteBehavior.Cascade),
				j => j
					.HasOne(ps => ps.Permission)
					.WithMany(p => p.PermissionSets)
					.HasForeignKey(ps => ps.PermissionId)
					.OnDelete(DeleteBehavior.Cascade)
			);

		modelBuilder.Entity<LogEntry>()
			.HasOne(le => le.LogEntryType)
			.WithMany(let => let.LogEntries)
			.HasForeignKey(le => le.LogEntryTypeId)
			.OnDelete(DeleteBehavior.Restrict);
	}

	public virtual DbSet<Account> Accounts { get; set; }
	public virtual DbSet<Permission> Permissions { get; set; }
	public virtual DbSet<Role> Roles { get; set; }
	public virtual DbSet<GlobalVariable> GlobalVariables { get; set; }
	public virtual DbSet<PermissionGrant> PermissionSets { get; set; }
	public virtual DbSet<LogEntry> LogEntries { get; set; }
	public virtual DbSet<LogEntryType> LogEntryTypes { get; set; }
}
