using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
			try
			{
				Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				clientSocket.Connect(new IPEndPoint(IPAddress.Parse("3.20.235.8"), 7771));
								
				string s = "{{\"cmd\":\"req_roomlist\"}}";
				byte[] jsoncmd = Encoding.UTF8.GetBytes(s);
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

				clientSocket.Close();



				Rooms = new List<string>();
				Rooms.Add("Machine 1");

			}
			catch
			{

			}
			
		}
	}
}