using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Data;
using WebAPI.Data.Models;
using WebAPI.Data.TransferObjects;

namespace WebAPI.Services;

public class AccountService {
	private readonly IConfiguration Configuration;
	private readonly ILogger<AccountService> Logger;
	private readonly DatabaseContext DatabaseContext;
	private readonly JwtService JwtService;
	private readonly Role DefaultRole;

	public AccountService(IConfiguration configuration, ILogger<AccountService> logger, DatabaseContext database, JwtService jwtService) {
		Configuration = configuration;
		DatabaseContext = database;
		Logger = logger;
		JwtService = jwtService;

		DefaultRole = GetDefaultRole();
	}

	private Role GetDefaultRole() {
		Role defaultRole = null;
		try {
			GlobalVariable gv = DatabaseContext.GlobalVariables.Single(gv => gv.Name == "DefaultRole");
			int roleID = int.Parse(gv.Value);

			defaultRole = DatabaseContext.Roles.Find(roleID);
		}
		catch (Exception ex) {
			Logger.LogCritical(ex, "Account creation will not be possible!");
		}

		return defaultRole;
	}

	public async Task<ServiceResult<string>> LoginAsync(LoginDTO loginDTO) {
		try {
			Account account = await GetAccount(username: loginDTO.Login, email: loginDTO.Login);
			if (account is null)
				return new(ResultType.NotFound);

			string password = JwtService.GetPasswordHash(loginDTO.Password, account.Salt);

			return password != account.Password
				? (new(ResultType.NotFound))
				: (new(ResultType.Ok) {
					Value = JwtService.CreateEncodedJwt(account.AccountID)
				});
		}
		catch (Exception ex) {
			Logger.LogError($"Login error: {ex.Message}\n{ex.StackTrace}");
			return new(ResultType.Exception);
		}
	}

	public async Task<ServiceResult<string>> RegisterAsync(RegisterDTO registerDTO) {
		try {
			if (await DatabaseContext.Accounts.Where(a => a.Email == registerDTO.Email).SingleOrDefaultAsync() != null)
				return new(ResultType.EmailTaken);
			else if (await DatabaseContext.Accounts.Where(a => a.Email == registerDTO.Email).SingleOrDefaultAsync() != null)
				return new(ResultType.UsernameTaken);

			var (Password, Salt) = EncryptPassword(registerDTO.Password.ToString());
			Account account = new() {
				Email = registerDTO.Email,
				Username = registerDTO.Username,
				Password = Password,
				Salt = Salt,
				RoleID = DefaultRole.RoleID
			};

			await DatabaseContext.Accounts.AddAsync(account);
			await DatabaseContext.SaveChangesAsync();
			return new(ResultType.Ok);
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Register error");
			return new(ResultType.Exception);
		}
	}

	public ServiceResult<string> Refresh(string token) {
		try {
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"]));
			uint tokenLifetime = Configuration.GetValue<uint>("JwtSettings:TokenLifetime");
			var handler = new JwtSecurityTokenHandler();
			JwtSecurityToken jwt = handler.ReadJwtToken(token);

			token = handler.CreateEncodedJwt(
				issuer: jwt.Issuer,
				audience: jwt.Audiences.First(),
				subject: new ClaimsIdentity(jwt.Claims),
				notBefore: DateTime.UtcNow,
				issuedAt: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddMinutes(tokenLifetime),
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha512)
			);

			return new(ResultType.Ok) {
				Value = token
			};
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Refresh error");
			return new(ResultType.Exception);
		}
	}

	public async Task<ServiceResult<object>> ChangePasswordAsync(PasswordDTO passwordDTO, string token) {
		try {
			Account account = await GetAccountFromToken(token);
			var oldPassword = EncryptPassword(passwordDTO.OldPassword.ToString(), account.Salt);
			var newPassword = EncryptPassword(passwordDTO.NewPassword.ToString());

			if (oldPassword.Password == account.Password) {
				account.Password = newPassword.Password;
				account.Salt = newPassword.Salt;
				DatabaseContext.Accounts.Update(account);
				DatabaseContext.SaveChanges();
				return new(ResultType.Ok);
			}

			return new(ResultType.InvalidPassword);
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Change password error");
			return new(ResultType.Exception);
		}
	}

	private async Task<Account> GetAccountFromToken(string token) {
		var handler = new JwtSecurityTokenHandler();
		JwtSecurityToken jwt = handler.ReadJwtToken(token);
		int accountID = int.Parse(jwt.Subject);

		return (await DatabaseContext.Accounts.FindAsync(accountID)) ?? throw new Exception("Couldn't find account with ID = " + accountID);
	}

	public async Task<ServiceResult<List<Permission>>> GetPermissions(int accountID) {
		throw new NotImplementedException();
		try {
			Account account = await DatabaseContext.Accounts.SingleAsync(a => a.AccountID == accountID);
			//Database.Permissions.
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Get permissions error");
			return new(ResultType.Exception);
		}
	}

	private static (string Password, string Salt) EncryptPassword(string password, string salt = null) {
		byte[] saltBytes = new byte[128 / 8];
		if (salt == null || salt == string.Empty) {
			using var rng = RandomNumberGenerator.Create();
			rng.GetNonZeroBytes(saltBytes);
			salt = Convert.ToBase64String(saltBytes);
		}
		else {
			saltBytes = Convert.FromBase64String(salt);
		}

		password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: password,
			salt: saltBytes,
			prf: KeyDerivationPrf.HMACSHA512,
			iterationCount: 100000,
			numBytesRequested: 512 / 8));

		return (password, salt);
	}

	private async Task<Account> GetAccount(int? accountID = null, string username = null, string email = null) {
		int argCount = 0;
		argCount = accountID is null ? argCount : argCount + 1;
		argCount = username is null ? argCount : argCount + 1;
		argCount = email is null ? argCount : argCount + 1;

		if (argCount == 0)
			throw new ArgumentException("At least one argument must be specified!");

		Account account = null;
		if (accountID is not null)
			account = await DatabaseContext.Accounts.FindAsync(accountID);

		if (username is not null && account is null)
			account = await DatabaseContext.Accounts.SingleOrDefaultAsync(a => a.Username == username);

		if (email is not null && account is null)
			account = await DatabaseContext.Accounts.SingleOrDefaultAsync(a => a.Email == email);

		return account;
	}
}
