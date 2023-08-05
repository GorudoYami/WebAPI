using Authentication.Common.Data.Forms;
using Authentication.Common.Data.Repositories;
using Authentication.Common.Enums;
using Authentication.Common.Services;
using Authentication.Common.Utils;
using Authentication.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Services;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AccountController : ControllerBase {
	private readonly ILogger<AccountController> _logger;
	private readonly IAccountRepository _accountRepo;
	private readonly IGlobalVariableRepository _globalVariableRepo;
	private readonly IJwtService _jwtService;
	private readonly ICryptoService _cryptoService;

	public AccountController(ILogger<AccountController> logger, IAccountRepository accountRepo, IGlobalVariableRepository globalVariableRepo, IJwtService jwtService, ICryptoService cryptoService) {
		_logger = logger;
		_accountRepo = accountRepo;
		_globalVariableRepo = globalVariableRepo;
		_jwtService = jwtService;
		_cryptoService = cryptoService;
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> LoginAsync(LoginData loginData) {
		Account account = await _accountRepo.GetAsync(loginData.Login);
		if (account == null) {
			return Unauthorized();
		}

		string hash = _cryptoService.GetPasswordHashBase64(loginData.Password, account.Salt);
		if (account.Password != hash) {
			return Unauthorized();
		}

		return Ok(_jwtService.CreateEncodedToken(account.Id));
	}

	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<IActionResult> Register(RegisterData registerData) {
		bool emailTaken = _accountRepo.ContainsEmail(registerData.Email);
		bool usernameTaken = _accountRepo.ContainsUsername(registerData.Username);

		if (emailTaken || usernameTaken) {
			return Conflict(emailTaken ? "Email already registered!" : "Username taken!");
		}

		GlobalVariable gv = await _globalVariableRepo.GetAsync(GlobalVariableType.DefaultRole);
		if (gv == null) {
			throw new Exception("DefaultRole is not specified in variables");
		}
		int defaultRoleId = gv.GetValue<int>();

		Account account = new() {
			Email = registerData.Email,
			Username = registerData.Username,
			Password = _cryptoService.GetPasswordHashBase64(registerData.Password, out string salt),
			Salt = salt,
			RoleId = defaultRoleId
		};
		await _accountRepo.AddAsync(account);

		return Ok();
	}

	[HttpGet("refresh")]
	public ActionResult Refresh() {
		string token = HttpContext.Request.Headers["token"];
		ServiceResult serviceResult = Accounts.Refresh(token);
		_ = int.TryParse(Jwt.GetSubject(token), out int accountID);

		return serviceResult.GetActionResult();
	}

	[HttpPost("change")]
	public async Task<ActionResult> ChangePassword(PasswordDTO passwordDTO) {
		string token = HttpContext.Request.Headers["token"];
		ServiceResult serviceResult = await Accounts.ChangePasswordAsync(passwordDTO, token);
		_ = int.TryParse(Jwt.GetSubject(token), out int accountID);

		return serviceResult.GetActionResult();
	}

	[HttpGet("permissions")]
	public async Task<ActionResult<List<Permission>>> Permissions([FromBody] int accountID) {
		throw new NotImplementedException();
	}
}
