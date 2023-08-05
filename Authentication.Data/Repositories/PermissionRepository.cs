using Authentication.Common.Data.Repositories;
using Authentication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Data.Repositories;
public class PermissionRepository : IPermissionRepository {
	private readonly DatabaseContext _database;

	public PermissionRepository(DatabaseContext database) {
		_database = database;
	}

	public async Task AddAsync(Permission permission) {
		_database.Permissions.Add(permission);
		await _database.SaveChangesAsync();
	}

	public async Task DeleteAsync(Permission permission) {
		_database.Permissions.Remove(permission);
		await _database.SaveChangesAsync();
	}

	public async Task UpdateAsync(Permission permission) {
		_database.Permissions.Update(permission);
		await _database.SaveChangesAsync();
	}
}
