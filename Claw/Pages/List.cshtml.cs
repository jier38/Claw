﻿using Microsoft.AspNetCore.Mvc;
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
				////Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				////clientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.43.76"), 8000));

				////byte[] date = new byte[1024];
				////int count = clientSocket.Receive(date);
				////string msg = Encoding.UTF8.GetString(date, 0, count);
				////Console.WriteLine(msg);

				////string s = "{\"cmd\":\"req_roomlist\"}";

				////clientSocket.Send(Encoding.UTF8.GetBytes(s));

				////count = clientSocket.Receive(date);
				////msg = Encoding.UTF8.GetString(date, 0, count);
				////Console.WriteLine(msg);

				////clientSocket.Close();
				///

				Rooms = new List<string>();
				Rooms.Add("Machine 1");

			}
			catch
			{

			}
			
		}
	}
}