using Authentication.Common.Data.Repositories;
using Authentication.Common.Utils;
using Authentication.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Data.Repositories;
public class AccountRepository : IAccountRepository {
	private readonly DatabaseContext _database;

	public AccountRepository(DatabaseContext database) {
		_database = database;
	}

	public async Task AddAsync(Account newAccount) {
		_database.Accounts.Add(newAccount);
		await _database.SaveChangesAsync();
	}

	public async Task DeleteAsync(Account account) {
		_database.Accounts.Remove(account);
		await _database.SaveChangesAsync();
	}

	public async Task UpdateAsync(Account account) {
		_database.Accounts.Update(account);
		await _database.SaveChangesAsync();
	}

	public async Task<Account> GetAsync(string login) {
		return await _database.Accounts.SingleOrDefaultAsync(x =>
			login.ToUpper() == (login.IsEmail() ? x.Email : x.Username));
	}

	public bool ContainsEmail(string email) {
		return _database.Accounts.Any(x => x.Email.ToUpper() == email.ToUpper());
	}

	public bool ContainsUsername(string username) {
		return _database.Accounts.Any(x => x.Username.ToUpper() == username.ToUpper());
	}
}
