using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Services.Interfaces;

public interface IServiceResult
{
	public ResultType Type { get; set; }
	public HttpStatusCode StatusCode { get; set; }
	public object Value { get; set; }
	public string Message { get; set; }

	public ActionResult GetActionResult();
}
