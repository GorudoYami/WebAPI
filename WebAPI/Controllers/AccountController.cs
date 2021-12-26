using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Data;
using WebAPI.Data.Models;
using WebAPI.Data.TransferObjects;
using WebAPI.Services;

namespace WebAPI.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class AccountController : ControllerBase {
		private readonly ILogger<AccountController> Logger;
        private readonly AccountService Accounts;
		public AccountController(ILogger<AccountController> logger, AccountService accounts) {
			Accounts = accounts;
			Logger = logger;
		}

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginDTO loginDTO) {
            Logger.LogInformation("POST Login");
            var token = await Accounts.LoginAsync(loginDTO);
            if (token == null) {
                Logger.LogInformation("User {Login} doesn't exist", loginDTO.Login);
                return NotFound();
            }
            else {
                Logger.LogInformation("User {Login} logged in. Token has been sent.", loginDTO.Login);
                return Ok(token);
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterDTO registerDTO) {
            Logger.LogInformation("[POST] Register");
            Logger.LogInformation("{Email} - {Username}");
            ResultType resultType = (await Accounts.RegisterAsync(registerDTO)).Type;
            if (resultType == ResultType.Ok) {
                Logger.LogInformation("New account created!");
                return NoContent();
            }
            else if (resultType == ResultType.EmailTaken) {
                Logger.LogInformation("Email is already in use!");
                return Conflict("Email is already in use!");
            }
            else if (resultType == ResultType.UsernameTaken) {
                Logger.LogInformation("Username is already in use!");
                return Conflict("Username is already in use!");
			}

            return Problem(statusCode: 500); 
        }

        [HttpPost("refresh")]
        public ActionResult Refresh([FromBody] string token) {
            Logger.LogInformation("[POST] Refresh");
            var result = Accounts.Refresh(token);

			return result.Type == ResultType.Ok ? Ok(result.Value) : Problem(statusCode: 500);
		}

        [HttpPost("changepassword")]
        public async Task<ActionResult> ChangePassword(PasswordDTO passwordDTO) {
            Logger.LogInformation("[POST] ChangePassword");
            var result = await Accounts.ChangePasswordAsync(passwordDTO, HttpContext.Request.Headers["token"]);

			return result.Type switch {
				ResultType.Ok => NoContent(),
				ResultType.InvalidPassword => NotFound(ResultType.InvalidPassword),
				_ => Problem(statusCode: 500)
			};
		}

        [HttpGet("permissions")]
        public async Task<ActionResult<List<Permission>>> Permissions([FromBody]int accountID) {
            
		}
	}
}
