using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Data.Models;

namespace WebAPI.Data {
	public class DatabaseContext : DbContext {
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

		public DbSet<Account> Accounts { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<GlobalVariable> GlobalVariables { get; set; }
		public DbSet<PermissionSet> PermissionSets { get; set; }
		public DbSet<LogEntry> LogEntries { get; set; }
		public DbSet<LogEntryType> LogEntryTypes { get; set; }
	}
}