using Authentication.Common.Enums;
using Authentication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Common.Data.Repositories;
public interface IGlobalVariableRepository {
	void AddAsync(GlobalVariable globalVariable);
	Task<GlobalVariable> GetAsync(GlobalVariableType globalVariableType);
	void UpdateAsync(GlobalVariable globalVariable);
}
