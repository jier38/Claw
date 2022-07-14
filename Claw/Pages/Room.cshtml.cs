using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Claw.Pages
{
	public class RoomModel : PageModel
	{
		private readonly ILogger<ListModel> _logger;
		private static Socket clientSocket;
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        string room;
		bool ret = false;
		int game_ret;
        private static String response = String.Empty;

        public RoomModel(ILogger<RoomModel> logger)
		{
			////_logger = logger;
		}

        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 256;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }


        public void OnGet()
		{
            room = Request.HttpContext.Request.QueryString.Value.Replace("?id=", "");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.SendTimeout = 0;
            clientSocket.ReceiveTimeout = 0;
    
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse("3.20.235.8"), 7771));
			Thread.Sleep(1000);
			if (clientSocket.Connected)
			{
				try
				{
                    //Thread SckSReceiveTd = new Thread(SckSReceiveProc);
                    //SckSReceiveTd.Start();
                    jsonmessage cmd = new jsonmessage() { cmd = "req_roomlist" };
                    string result = SendOperation(cmd);
                    int intAcceptData;
                    byte[] data = new byte[1024];
                    intAcceptData = clientSocket.Receive(data);
                    cmd = new jsonmessage() { cmd = "enter_room", mac = room };
					result = SendOperation(cmd);
				}
				catch (Exception ex)
				{

				}
			}
        }


        public string SendOperation(jsonmessage cmd)
		{
			string msg = "";

			string ss = JsonSerializer.Serialize(cmd);
			byte[] jsoncmd = Encoding.UTF8.GetBytes(ss);
			byte[] smsg = new byte[3 + jsoncmd.Length];
			smsg[0] = (byte)0xda;
			smsg[1] = (byte)(jsoncmd.Length / 256);
			smsg[2] = (byte)(jsoncmd.Length % 256);
			jsoncmd.CopyTo(smsg, 3);
			clientSocket.Send(smsg);

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
            int intAcceptData;
            byte[] data = new byte[1024];
            intAcceptData = clientSocket.Receive(data);
            Thread.Sleep(2000);
			return Content(DateTime.Now.Ticks.ToString() + ": Game Start");
		}

		public IActionResult OnGetUp()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "operation", type= 1};
			string result = SendOperation(cmd);
			Thread.Sleep(500);
            cmd = new jsonmessage() { cmd = "operation", type = 5 };
            result = SendOperation(cmd);
            return Content(DateTime.Now.Ticks.ToString() + ":Up");
		}

		public IActionResult OnGetDown()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "operation", type = 0 };
			string result = SendOperation(cmd);
            Thread.Sleep(500);
            cmd = new jsonmessage() { cmd = "operation", type = 5 };
            result = SendOperation(cmd);
            return Content(DateTime.Now.Ticks.ToString() + ":Down");
		}
		public IActionResult OnGetLeft()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "operation", type = 2 };
			string result = SendOperation(cmd);
            Thread.Sleep(500);
            cmd = new jsonmessage() { cmd = "operation", type = 5 };
            result = SendOperation(cmd);
            return Content(DateTime.Now.Ticks.ToString() + ":Left");
		}
		public IActionResult OnGetRight()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "operation", type = 3 };
			string result = SendOperation(cmd);
            Thread.Sleep(500);
            cmd = new jsonmessage() { cmd = "operation", type = 5 };
            result = SendOperation(cmd);
            return Content(DateTime.Now.Ticks.ToString() + ":Right");
		}

		public IActionResult OnGetDrop()
		{
			jsonmessage cmd = new jsonmessage() { cmd = "operation", type = 4 };
            string msg = "";
			ret = false;
            string ss = JsonSerializer.Serialize(cmd);
            byte[] jsoncmd = Encoding.UTF8.GetBytes(ss);
            byte[] smsg = new byte[3 + jsoncmd.Length];
            smsg[0] = (byte)0xda;
            smsg[1] = (byte)(jsoncmd.Length / 256);
            smsg[2] = (byte)(jsoncmd.Length % 256);
            jsoncmd.CopyTo(smsg, 3);
            clientSocket.Send(smsg);
            Thread.Sleep(1000);
            int intAcceptData;
            byte[] data = new byte[1024];
            intAcceptData = clientSocket.Receive(data);
            if (intAcceptData > 0)
            {

                // 往下就自己寫接收到來自Client端的資料後要做什麼事唄~^^”
                msg = Encoding.UTF8.GetString(data, 0, intAcceptData);
                Console.WriteLine(msg);
                msg = "{" + msg.Split("{")[1];
                if (msg.IndexOf("cmd") > 0)
                {
                    jsonmessage result = JsonSerializer.Deserialize<jsonmessage>(msg);
                    switch (result.cmd)
                    {
                        case "game_ret":

                            game_ret = result.ret;
                            ret = true;
                            break;
                    }
                }
            }


            return Content(DateTime.Now.Ticks.ToString() + (game_ret==1 ? ":You win" : ":You loss"));
		}

		public IActionResult OnGetClose()
        {
			jsonmessage cmd = new jsonmessage() { cmd = "exit_room" };
			string result = SendOperation(cmd);
			clientSocket.Close();
			return Content(DateTime.Now.Ticks.ToString() + "Room Close");
		}

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private void SckSReceiveProc()
        {
                while (clientSocket != null && clientSocket.Connected)
                {
                    try
                    {
                        string strAcceptData = string.Empty;
                        int intAcceptData;
                        byte[] data = new byte[1024];
                        intAcceptData = clientSocket.Receive(data);
						if (intAcceptData > 0)
						{

							// 往下就自己寫接收到來自Client端的資料後要做什麼事唄~^^”
							string msg = Encoding.UTF8.GetString(data, 0, intAcceptData);
							Console.WriteLine(msg);
                            msg = "{" + msg.Split("{")[1];
							if (msg.IndexOf("cmd") > 0)
							{
								jsonmessage result = JsonSerializer.Deserialize<jsonmessage>(msg);
								switch (result.cmd)
                            {
									case "start_game":


										break;
									case "game_ret":

										game_ret = result.ret;
										ret = true;
										break;



								}
							}
						}
                     

                    }
                    catch
                    {

                    }
                   

				}
        }
    }
}