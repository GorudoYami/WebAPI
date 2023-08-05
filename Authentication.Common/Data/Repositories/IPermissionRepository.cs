using Authentication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Common.Data.Repositories;
public interface IPermissionRepository {
	public Task AddAsync(Permission permission);
	public Task DeleteAsync(Permission permission);
	public Task UpdateAsync(Permission permission);
}
