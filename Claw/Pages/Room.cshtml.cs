using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Claw.Pages
{
	public class RoomModel : PageModel
	{
		private readonly ILogger<ListModel> _logger;

		public RoomModel(ILogger<RoomModel> logger)
		{
			////_logger = logger;
		}

		public void OnGet()
		{

		}

		/* 
			{"cmd":"req_roomlist"}
			{"cmd":"enter_room","mac":"XXXX"}
			{"cmd":"exit_room"}
			{"cmd":"start_game"}
			{"cmd":"operation","type":0}
		*/
		public IActionResult OnGetStart()
		{
			return Content(DateTime.Now.Ticks.ToString() + ":AjaxGet");
		}

		public IActionResult OnGetUp()
		{
			return Content(DateTime.Now.Ticks.ToString() + ":AjaxGet");
		}

		public IActionResult OnGetDown()
		{
			return Content(DateTime.Now.Ticks.ToString() + ":AjaxGet");
		}
		public IActionResult OnGetLeft()
		{
			return Content(DateTime.Now.Ticks.ToString() + ":AjaxGet");
		}
		public IActionResult OnGetRight()
		{
			return Content(DateTime.Now.Ticks.ToString() + ":AjaxGet");
		}
		public IActionResult OnGetDrop()
		{
			return Content(DateTime.Now.Ticks.ToString() + ":AjaxGet");
		}
		public IActionResult OnGetClose()
		{
			return Content(DateTime.Now.Ticks.ToString() + ":AjaxGet");
		}
	}
}