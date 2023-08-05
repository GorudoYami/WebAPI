using Authentication.Common.Data.Repositories;
using Authentication.Common.Enums;
using Authentication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Data.Repositories;
public class GlobalVariableRepository : IGlobalVariableRepository {
	private readonly DatabaseContext _database;

	public GlobalVariableRepository(DatabaseContext database) {
		_database = database;
	}

	public async void AddAsync(GlobalVariable globalVariable) {
		await _database.GlobalVariables.AddAsync(globalVariable);
		await _database.SaveChangesAsync();
	}

	public async void UpdateAsync(GlobalVariable globalVariable) {
		_database.GlobalVariables.Update(globalVariable);
		await _database.SaveChangesAsync();
	}

	public async Task<GlobalVariable> GetAsync(GlobalVariableType globalVariableType) {
		return await _database.GlobalVariables.FindAsync(globalVariableType);
	}
}
