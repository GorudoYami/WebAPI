using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Models;

namespace WebAPI.Data;

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
			.HasForeignKey(a => a.RoleID)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Permission>()
			.HasMany(p => p.Roles)
			.WithMany(r => r.Permissions)
			.UsingEntity<PermissionSet>
			(
				j => j
					.HasOne(ps => ps.Role)
					.WithMany(r => r.PermissionSets)
					.HasForeignKey(ps => ps.RoleID)
					.OnDelete(DeleteBehavior.Cascade),
				j => j
					.HasOne(ps => ps.Permission)
					.WithMany(p => p.PermissionSets)
					.HasForeignKey(ps => ps.PermissionID)
					.OnDelete(DeleteBehavior.Cascade)
			);

		modelBuilder.Entity<LogEntry>()
			.HasOne(le => le.LogEntryType)
			.WithMany(let => let.LogEntries)
			.HasForeignKey(le => le.LogEntryTypeID)
			.OnDelete(DeleteBehavior.Restrict);
	}

	public virtual DbSet<Account> Accounts { get; set; }
	public virtual DbSet<Permission> Permissions { get; set; }
	public virtual DbSet<Role> Roles { get; set; }
	public virtual DbSet<GlobalVariable> GlobalVariables { get; set; }
	public virtual DbSet<PermissionSet> PermissionSets { get; set; }
	public virtual DbSet<LogEntry> LogEntries { get; set; }
	public virtual DbSet<LogEntryType> LogEntryTypes { get; set; }
}
