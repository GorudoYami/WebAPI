﻿using System;
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

namespace WebAPI.Services {
	public class AccountService {
		private readonly Database Database;
		private readonly IConfiguration Configuration;
		private readonly ILogger<AccountService> Logger;

		public AccountService(IConfiguration configuration, Database database, ILogger<AccountService> logger) {
			Configuration = configuration;
			Database = database;
			Logger = logger;
		}

		public async Task<ServiceResult<string>> LoginAsync(LoginDTO loginDTO) {
			try {
				SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"]));
				uint tokenLifetime = Configuration.GetValue<uint>("JwtSettings:TokenLifetime");

				Account account = await Database.Accounts.Where(a => a.Email == loginDTO.Login).SingleOrDefaultAsync();
				if (account is null)
					account = await Database.Accounts.Where(a => a.Username == loginDTO.Login).SingleOrDefaultAsync();

				if (account is null)
					return new(ResultType.NotFound);

				byte[] salt = Convert.FromBase64String(account.Salt);
				string password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: loginDTO.Password,
					salt: salt,
					prf: KeyDerivationPrf.HMACSHA512,
					iterationCount: 100000,
					numBytesRequested: 512 / 8));

				if (password != account.Password)
					return new(ResultType.NotFound);

				List<Claim> claims = new() {
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim(JwtRegisteredClaimNames.Sub, account.AccountID.ToString())
				};

				JwtSecurityTokenHandler handler = new();

				string token = handler.CreateEncodedJwt(
					issuer: Configuration["JwtSettings:Issuer"],
					audience: Configuration["JwtSettings:Audience"],
					subject: new ClaimsIdentity(claims),
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
				Logger.LogError("Exception occured in {Location}: {Message}\n{StackTrace}", ex.Source, ex.Message, ex.StackTrace);
				return new(ResultType.Exception);
			}
		}

		public async Task<ServiceResult<string>> RegisterAsync(RegisterDTO registerDTO) {
			try {
				if (await Database.Accounts.Where(a => a.Email == registerDTO.Email).SingleOrDefaultAsync() != null)
					return new(ResultType.EmailTaken);
				else if (await Database.Accounts.Where(a => a.Email == registerDTO.Email).SingleOrDefaultAsync() != null)
					return new(ResultType.UsernameTaken);

				var (Password, Salt) = EncryptPassword(registerDTO.Password);
				Account account = new() {
					Email = registerDTO.Email,
					Username = registerDTO.Username,
					Password = Password,
					Salt = Salt,
					RoleID = 5  // Default group - change to something like a static class that loads Roles to fields
				};


				await Database.Accounts.AddAsync(account);
				await Database.SaveChangesAsync();
				return new(ResultType.Ok);
			}
			catch (Exception ex) {
				Logger.LogError("Exception occured in {Location}: {Message}\n{StackTrace}", ex.Source, ex.Message, ex.StackTrace);
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
				Logger.LogError("Exception occured in {Location}: {Message}\n{StackTrace}", ex.Source, ex.Message, ex.StackTrace);
				return new(ResultType.Exception);
			}
		}

		public async Task<ServiceResult<object>> ChangePasswordAsync(PasswordDTO passwordDTO, string token) {
			try {
				Account account = await GetAccountFromToken(token);
				var oldPassword = EncryptPassword(passwordDTO.OldPassword, account.Salt);
				var newPassword = EncryptPassword(passwordDTO.NewPassword);

				if (oldPassword.Password == account.Password) {
					account.Password = newPassword.Password;
					account.Salt = newPassword.Salt;
					Database.Accounts.Update(account);
					Database.SaveChanges();
					return new(ResultType.Ok);
				}

				return new(ResultType.InvalidPassword);
			}
			catch (Exception ex) {
				Logger.LogError("Exception occured in {Location}: {Message}\n{StackTrace}", ex.Source, ex.Message, ex.StackTrace);
				return new(ResultType.Exception);
			}
		}

		private async Task<Account> GetAccountFromToken(string token) {
			var handler = new JwtSecurityTokenHandler();
			JwtSecurityToken jwt = handler.ReadJwtToken(token);
			int accountID = int.Parse(jwt.Subject);

			return (await Database.Accounts.FindAsync(accountID)) ?? throw new Exception("Couldn't find account with ID = " + accountID);
		}

		private static (string Password, string Salt) EncryptPassword(string password, string salt = null) {
			byte[] saltBytes = new byte[128 / 8];
			if (salt == null || salt == string.Empty) {
				using RNGCryptoServiceProvider rng = new();
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

		public async Task<ServiceResult<List<Permission>>> GetPermissions(int accountID) {
			try {
				Account account = await Database.Accounts.SingleAsync(a => a.AccountID == accountID);
				//Database.Permissions.
			}
			catch (Exception ex) {
				Logger.LogError("Exception occured in {Location}: {Message}\n{StackTrace}", ex.Source, ex.Message, ex.StackTrace);
				return new(ResultType.Exception);
			}
		}
	}
}