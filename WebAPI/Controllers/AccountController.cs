using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Data;
using WebAPI.Data.Models;
using WebAPI.Data.TransferObjects;
using WebAPI.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountController : ControllerBase {
	private readonly ILogger<AccountController> Logger;
	private readonly AccountService Accounts;
	private readonly JwtService Jwt;

	public AccountController(ILogger<AccountController> logger, AccountService accounts, JwtService jwt) {
		Logger = logger;
		Accounts = accounts;
		Jwt = jwt;
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<ActionResult> Login(LoginDTO loginDTO) {
		ServiceResult serviceResult = await Accounts.LoginAsync(loginDTO);

		Logger.LogInformation(
			"[POST] Login: User {Login} | Result {Result} | ServiceResult {ServiceResult}",
			loginDTO.Login,
			serviceResult.StatusCode,
			serviceResult.Type);

		return serviceResult.GetActionResult();
	}

	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<ActionResult> Register(RegisterDTO registerDTO) {
		ServiceResult serviceResult = await Accounts.RegisterAsync(registerDTO);

		Logger.LogInformation(
			"[POST] Register: User {Username} Email {Email} | StatusCode {StatusCode} | ServiceResult {Type}",
			registerDTO.Username,
			registerDTO.Email,
			serviceResult.StatusCode,
			serviceResult.Type);

		return serviceResult.GetActionResult();
	}

	[HttpGet("refresh")]
	public ActionResult Refresh() {
		string token = HttpContext.Request.Headers["token"];
		ServiceResult serviceResult = Accounts.Refresh(token);
		_ = int.TryParse(Jwt.GetSubject(token), out int accountID);

		Logger.LogInformation(
			"[POST] Refresh: AccountID {AccountID} | Result {StatusCode} | ServiceResult {Type}",
			accountID,
			serviceResult.StatusCode,
			serviceResult.Type);

		return serviceResult.GetActionResult();
	}

	[HttpPost("changepassword")]
	public async Task<ActionResult> ChangePassword(PasswordDTO passwordDTO) {
		string token = HttpContext.Request.Headers["token"];
		ServiceResult serviceResult = await Accounts.ChangePasswordAsync(passwordDTO, token);
		_ = int.TryParse(Jwt.GetSubject(token), out int accountID);

		Logger.LogInformation(
			"[POST] ChangePassword: AccountID {AccountID} | Result {StatusCode} | ServiceResult {Type}",
			accountID,
			serviceResult.StatusCode,
			serviceResult.Type);

		return serviceResult.GetActionResult();
	}

	[HttpGet("permissions")]
	public async Task<ActionResult<List<Permission>>> Permissions([FromBody] int accountID) {
		throw new NotImplementedException();
	}
}
