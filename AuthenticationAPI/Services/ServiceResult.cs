using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services;

public enum ResultType {
	OK,
	UsernameTaken,
	EmailTaken,
	NotFound,
	InvalidPassword,
	Exception
}

public class ServiceResult : IServiceResult {
	public ResultType Type { get; set; }
	public HttpStatusCode StatusCode { get; set; }
	public object Value { get; set; }
	public string Message { get; set; }

	public ServiceResult(ResultType result) {
		Type = result;
		RefreshMessage();
		RefreshStatusCode();
	}

	private void RefreshMessage() {
		Message = Type switch {
			ResultType.OK => "OK",
			ResultType.UsernameTaken => "Username taken",
			ResultType.EmailTaken => "Email taken",
			ResultType.NotFound => "Entity doesn't exist",
			ResultType.InvalidPassword => "Invalid password",
			ResultType.Exception => "Exception occured",
			_ => null
		};
	}

	private void RefreshStatusCode() {
		StatusCode = Type switch {
			ResultType.OK => HttpStatusCode.OK,
			ResultType.UsernameTaken => HttpStatusCode.Conflict,
			ResultType.EmailTaken => HttpStatusCode.Conflict,
			ResultType.InvalidPassword => HttpStatusCode.NotFound,
			ResultType.NotFound => HttpStatusCode.NotFound,
			ResultType.Exception => HttpStatusCode.InternalServerError,
			_ => HttpStatusCode.InternalServerError
		};
	}

	public ActionResult GetActionResult() {
		return StatusCode == HttpStatusCode.OK
			? new ObjectResult(Value) { StatusCode = (int)StatusCode }
			: new ObjectResult(Message) { StatusCode = (int)StatusCode };
	}
}
