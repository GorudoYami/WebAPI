using Authentication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Common.Data.Repositories;
public interface IAccountRepository {
	Task AddAsync(Account newAccount);
	Task UpdateAsync(Account account);
	Task DeleteAsync(Account account);
	Task<Account> GetAsync(string login);
	bool ContainsEmail(string email);
	bool ContainsUsername(string username);
}
