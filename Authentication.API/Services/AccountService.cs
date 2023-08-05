using Authentication.Common.Data.Forms;
using Authentication.Common.Data.Repositories;
using Authentication.Common.Enums;
using Authentication.Data;
using Authentication.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Services.Interfaces;

namespace Authentication.API.Services;

public class AccountService : IAccountService {
	private readonly ILogger<IAccountService> _logger;
	private readonly IJwtService _jwtService;
	private readonly IGlobalVariableRepository _globalVariableRepo;
	private readonly IAccountRepository _accountRepo;
	private readonly IRoleRepository _roleRepo;
	private readonly Role _defaultRole;

	public AccountService(ILogger<IAccountService> logger, IJwtService jwtService) {
		_logger = logger;
		_jwtService = jwtService;
		_defaultRole = GetDefaultRoleAsync().GetAwaiter().GetResult();
	}

	private async Task<Role> GetDefaultRoleAsync() {
		GlobalVariable gv = await _globalVariableRepo.GetAsync(GlobalVariableType.DefaultRole);
		if (gv == null) {
			throw new InvalidOperationException("DefaultRole variable not defined in database");
		}

		Role defaultRole = await _roleRepo.GetAsync(gv.GetValue<int>());
		if (defaultRole == null) {
			throw new InvalidOperationException("Role specified by DefaultRole variable does not exist");
		}

		return defaultRole;
	}

	public async Task<ServiceResult> LoginAsync(LoginData loginData) {
		try {
			Account account = await GetAccount(username: loginData.Login, email: loginData.Login);
			if (account is null) {
				return new(ResultType.NotFound);
			}

			string hash = _jwtService.GetPasswordHash(loginData.Password, account.Salt);

			return hash != account.Password
				? (new(ResultType.NotFound))
				: (new(ResultType.OK) {
					Value = _jwtService.CreateEncodedToken(account.Id)
				});
		}
		catch (Exception ex) {
			_logger.LogError(ex, "Login error: {Error}\n{StackTrace}", ex.Message, ex.StackTrace);
			return new(ResultType.Exception);
		}
	}

	public async Task<ServiceResult> RegisterAsync(RegisterDTO registerDTO) {
		try {
			if (await Database.Accounts.Where(a => a.Email == registerDTO.Email).SingleOrDefaultAsync() != null) {
				return new(ResultType.EmailTaken);
			}
			else if (await Database.Accounts.Where(a => a.Email == registerDTO.Email).SingleOrDefaultAsync() != null) {
				return new(ResultType.UsernameTaken);
			}

			(string hash, string salt) = _jwtService.GetPasswordHash(registerDTO.Password);
			Account account = new() {
				Email = registerDTO.Email,
				Username = registerDTO.Username,
				Password = hash,
				Salt = salt,
				RoleID = DefaultRole.RoleID
			};

			await Database.Accounts.AddAsync(account);
			await Database.SaveChangesAsync();
			return new(ResultType.OK);
		}
		catch (Exception ex) {
			_logger.LogError(ex, "Register error: {Error}\n{StackTrace}", ex.Message, ex.StackTrace);
			return new(ResultType.Exception);
		}
	}

	public ServiceResult Refresh(string token) {
		try {
			int accountID = 0;

			return new(ResultType.OK) {
				Value = _jwtService.CreateEncodedToken(accountID)
			};
		}
		catch (Exception ex) {
			_logger.LogError(ex, "Refresh error: {Error}\n{StackTrace}", ex.Message, ex.StackTrace);
			return new(ResultType.Exception);
		}
	}

	public async Task<ServiceResult> ChangePasswordAsync(PasswordDTO passwordDTO, string token) {
		try {
			int accountID = int.Parse(_jwtService.GetSubject(token));
			Account account = await Database.Accounts.FindAsync(accountID);

			string oldHash = _jwtService.GetPasswordHash(passwordDTO.OldPassword, account.Salt);
			(string newHash, string newSalt) = _jwtService.GetPasswordHash(passwordDTO.NewPassword);

			if (oldHash == account.Password) {
				account.Password = newHash;
				account.Salt = newSalt;
				Database.Accounts.Update(account);
				Database.SaveChanges();
				return new(ResultType.OK);
			}

			return new(ResultType.InvalidPassword);
		}
		catch (Exception ex) {
			_logger.LogError(ex, "ChangePassword error: {Error}\n{StackTrace}", ex.Message, ex.StackTrace);
			return new(ResultType.Exception);
		}
	}

	public async Task<ServiceResult> GetPermissionsAsync(int accountID) {
		throw new NotImplementedException();
		try {
			Account account = await Database.Accounts.SingleAsync(a => a.Id == accountID);
			//Database.Permissions.
		}
		catch (Exception ex) {
			_logger.LogError(ex, "GetPermissions error: {Error}\n{StackTrace}", ex.Message, ex.StackTrace);
			return new(ResultType.Exception);
		}
	}

	private async Task<Account> GetAccount(int? accountID = null, string username = null, string email = null) {
		int argCount = 0;
		argCount = accountID is null ? argCount : argCount + 1;
		argCount = username is null ? argCount : argCount + 1;
		argCount = email is null ? argCount : argCount + 1;

		if (argCount == 0) {
			throw new ArgumentException("At least one argument must be specified!");
		}

		Account account = null;
		if (accountID is not null) {
			account = await Database.Accounts.FindAsync(accountID);
		}

		if (username is not null && account is null) {
			account = await Database.Accounts.SingleOrDefaultAsync(a => a.Username == username);
		}

		if (email is not null && account is null) {
			account = await Database.Accounts.SingleOrDefaultAsync(a => a.Email == email);
		}

		return account;
	}
}
