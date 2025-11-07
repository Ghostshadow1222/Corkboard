using Microsoft.AspNetCore.Mvc;

namespace Corkboard.Controllers;
public class ChatController : Controller
{
	[HttpGet("chat/{id?}")]
	public IActionResult Index(string? id = null)
	{
		// Expose the optional id to the view for later use.
		ViewData["ChatId"] = id;
		return View();
	}
}
