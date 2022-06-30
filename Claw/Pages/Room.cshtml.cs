using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Claw.Pages
{
	public class RoomModel : PageModel
	{
		private readonly ILogger<ListModel> _logger;
		private Socket clientSocket;

		public RoomModel(ILogger<RoomModel> logger)
		{
			////_logger = logger;
		}

		public void OnGet()
		{
			string room = Request.HttpContext.Request.QueryString.Value.Replace("?id=","");
			clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			clientSocket.Connect(new IPEndPoint(IPAddress.Parse("3.20.235.8"), 7771));

			jsonmessage cmd = new jsonmessage() { cmd = "enter_room", mac = room };
			string result = SendOperation(cmd); 
			

		}

		public string SendOperation(jsonmessage cmd)
		{
			string ss = JsonSerializer.Serialize(cmd);
			byte[] jsoncmd = Encoding.UTF8.GetBytes(ss);
			byte[] smsg = new byte[3 + jsoncmd.Length];
			smsg[0] = (byte)0xda;
			smsg[1] = (byte)(jsoncmd.Length / 256);
			smsg[2] = (byte)(jsoncmd.Length % 256);
			jsoncmd.CopyTo(smsg, 3);
			clientSocket.Send(smsg);

			byte[] date = new byte[1024];
			int count = clientSocket.Receive(date);
			string msg = Encoding.UTF8.GetString(date, 0, count);
			Console.WriteLine(msg);

			return msg;
		}

		/* 
			{"cmd":"req_roomlist"}
			{"cmd":"enter_room","mac":"XXXX"}
			{"cmd":"exit_room"}
			{"cmd":"start_game"}
			{"cmd":"operation","type":0}
		game_ret ret
		*/
		public IActionResult OnGetStart()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "start_game" };
			string result = SendOperation(cmd);
			return Content(DateTime.Now.Ticks.ToString() + ": Game Start");
		}

		public IActionResult OnGetUp()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "operation", type= 1};
			string result = SendOperation(cmd);
			return Content(DateTime.Now.Ticks.ToString() + ":Up");
		}

		public IActionResult OnGetDown()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "operation", type = 0 };
			string result = SendOperation(cmd);
			return Content(DateTime.Now.Ticks.ToString() + ":Down");
		}
		public IActionResult OnGetLeft()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "operation", type = 2 };
			string result = SendOperation(cmd);
			return Content(DateTime.Now.Ticks.ToString() + ":Left");
		}
		public IActionResult OnGetRight()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "operation", type = 3 };
			string result = SendOperation(cmd);
			return Content(DateTime.Now.Ticks.ToString() + ":Right");
		}

		public IActionResult OnGetDrop()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "operation", type = 4 };
			string result = SendOperation(cmd);
			return Content(DateTime.Now.Ticks.ToString() + ":Drop");
		}
		public IActionResult OnGetClose()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "exit_room" };
			string result = SendOperation(cmd);
			clientSocket.Close();
			return Content(DateTime.Now.Ticks.ToString() + "Room Close");
		}
	}
}