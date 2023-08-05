using Authentication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Common.Data.Repositories;
public interface IRoleRepository {
	Task AddAsync(Role role);
	Task DeleteAsync(Role role);
	Task UpdateAsync(Role role);
	Task<Role> GetAsync(int roleId);
}
