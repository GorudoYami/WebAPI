using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Models;
using WebAPI.Data.TransferObjects;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services;

public class AccountService : IAccountService {
	private ILogger<IAccountService> Logger { get; set; }
	protected DatabaseContext Database { get; set; }
	private IJwtService Jwt { get; set; }
	private Role DefaultRole { get; set; }

	public AccountService(ILogger<IAccountService> logger, DatabaseContext database, IJwtService jwtService) {
		Database = database;
		Logger = logger;
		Jwt = jwtService;

		DefaultRole = GetDefaultRole();
	}

	private Role GetDefaultRole() {
		Role defaultRole = null;
		try {
			GlobalVariable gv = Database.GlobalVariables.Single(gv => gv.Name == "DefaultRole");
			int roleID = int.Parse(gv.Value);

			defaultRole = Database.Roles.Find(roleID);
		}
		catch (Exception ex) {
			Logger.LogCritical(ex, "{Error}\n{StackTrace}\nAccount creation will not be possible!", ex.Message, ex.StackTrace);
		}

		return defaultRole;
	}

	public async Task<ServiceResult> LoginAsync(LoginDTO loginDTO) {
		try {
			Account account = await GetAccount(username: loginDTO.Login, email: loginDTO.Login);
			if (account is null) {
				return new(ResultType.NotFound);
			}

			string hash = Jwt.GetPasswordHash(loginDTO.Password, account.Salt);

			return hash != account.Password
				? (new(ResultType.NotFound))
				: (new(ResultType.OK) {
					Value = Jwt.CreateEncodedJwt(account.AccountID)
				});
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Login error: {Error}\n{StackTrace}", ex.Message, ex.StackTrace);
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

			(string hash, string salt) = Jwt.GetPasswordHash(registerDTO.Password);
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
			Logger.LogError(ex, "Register error: {Error}\n{StackTrace}", ex.Message, ex.StackTrace);
			return new(ResultType.Exception);
		}
	}

	public ServiceResult Refresh(string token) {
		try {
			int accountID = 0;

			return new(ResultType.OK) {
				Value = Jwt.CreateEncodedJwt(accountID)
			};
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Refresh error: {Error}\n{StackTrace}", ex.Message, ex.StackTrace);
			return new(ResultType.Exception);
		}
	}

	public async Task<ServiceResult> ChangePasswordAsync(PasswordDTO passwordDTO, string token) {
		try {
			int accountID = int.Parse(Jwt.GetSubject(token));
			Account account = await Database.Accounts.FindAsync(accountID);

			string oldHash = Jwt.GetPasswordHash(passwordDTO.OldPassword, account.Salt);
			(string newHash, string newSalt) = Jwt.GetPasswordHash(passwordDTO.NewPassword);

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
			Logger.LogError(ex, "ChangePassword error: {Error}\n{StackTrace}", ex.Message, ex.StackTrace);
			return new(ResultType.Exception);
		}
	}

	public async Task<ServiceResult> GetPermissionsAsync(int accountID) {
		throw new NotImplementedException();
		try {
			Account account = await Database.Accounts.SingleAsync(a => a.AccountID == accountID);
			//Database.Permissions.
		}
		catch (Exception ex) {
			Logger.LogError(ex, "GetPermissions error: {Error}\n{StackTrace}", ex.Message, ex.StackTrace);
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
