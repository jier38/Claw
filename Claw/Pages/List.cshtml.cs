using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Claw.Pages
{
	public class ListModel : PageModel
	{
		public List<string> Rooms;
		private readonly ILogger<ListModel> _logger;

		public ListModel(ILogger<ListModel> logger)
		{
			//_logger = logger;
		}

		public void OnGet()
		{
			string msg;
			try
			{
				Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				clientSocket.Connect(new IPEndPoint(IPAddress.Parse("3.20.235.8"), 7771));

				jsonmessage s = new jsonmessage() { cmd = "req_roomlist" };
				string ss = JsonSerializer.Serialize(s);
				byte[] jsoncmd = Encoding.UTF8.GetBytes(ss);
				byte[] smsg = new byte[3 + jsoncmd.Length];
				smsg[0] = (byte)0xda;
				smsg[1] = (byte)(jsoncmd.Length / 256);
				smsg[2] = (byte)(jsoncmd.Length % 256);
				jsoncmd.CopyTo(smsg, 3);
				clientSocket.Send(smsg);

				byte[] date = new byte[1024];
				int count = clientSocket.Receive(date);
				msg = Encoding.UTF8.GetString(date, 0, count);
				Console.WriteLine(msg);
				clientSocket.Close();
				msg = "{" + msg.Split("{")[1];
				//msg = "{" + "\"rooms\":[\"ACF1E188BE65\"],\"cmd\":\"reply_roomlist\"" + "}";
				jsonmessage result = JsonSerializer.Deserialize<jsonmessage>(msg);
				Rooms = new List<string>();
				foreach (string r in result.rooms)
				{
					Rooms.Add(r);
				}							
			}
			catch
			{

			}
			
		}
	}

	public class jsonmessage
    {
        //room list reply:{"rooms":["ACF1E188BE65"],"cmd":"reply_roomlist"}
        //jsstr{"cmd":"enter_room","mac":"ACF1E188BE65"}
        //jsstr{"cmd":"start_game"}
        //jsstr{"cmd":"operation","type":1}
        //jsstr{"cmd":"operation","type":4} 抓
        //jsstr{"cmd":"exit_room"}

        public string cmd { set; get; }
        public int type { set; get; }
		public string mac { set; get; }
		public string[] rooms { set; get; }
    }
}