using Authentication.Common.Data.Repositories;
using Authentication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Data.Repositories;
public class RoleRepository : IRoleRepository {
	private readonly DatabaseContext _database;

	public RoleRepository(DatabaseContext database) {
		_database = database;
	}
	public async Task AddAsync(Role role) {
		_database.Roles.Add(role);
		await _database.SaveChangesAsync();
	}

	public async Task DeleteAsync(Role role) {
		_database.Roles.Remove(role);
		await _database.SaveChangesAsync();
	}

	public async Task UpdateAsync(Role role) {
		_database.Roles.Update(role);
		await _database.SaveChangesAsync();
	}

	public async Task<Role> GetAsync(int roleId) {
		return await _database.Roles.FindAsync(roleId);
	}
}
