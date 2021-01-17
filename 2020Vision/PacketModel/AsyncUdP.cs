using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Vision2020
{

	public class UDPSocket : IDisposable
	{
		private Socket _socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
		private const int bufSize = 50 * 1024;
		private State state = new State();
		private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
		private AsyncCallback recv = null;

		static Queue<byte[]> message = new Queue<byte[]>();

		public byte[] NextMessage()
		{
			lock (message)
			{
				if (message.Count >= 1)
				{
					return message.Dequeue();
				}
			}
			return null;
		}


		public class State
		{
			public byte[] buffer = new byte[bufSize];
		}

		public static IPAddress[] GetLocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			List<IPAddress> result = new List<IPAddress>();
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					result.Add(ip);
				}
			}
			return (result.ToArray());
		}

		public void Server(int port)
		{
			_socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
			_socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReceiveBuffer, 10000000);
			//_socket.Bind(new IPEndPoint(GetLocalIPAddress(), port));
			Receive();
		}

		public void Client(int port)
		{
			_socket.Bind(new IPEndPoint(IPAddress.Any, port));
			Receive();
		}

		//public void Send(string text)
		//{
		//    byte[] data = Encoding.ASCII.GetBytes(text);
		//    _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
		//    {
		//        State so = (State)ar.AsyncState;
		//        int bytes = _socket.EndSend(ar);
		//        WriteToFile("SEND: " + text);
		//    }, state);
		//}

		private void Receive()
		{
			var canStart =
			_socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
			{
				try
				{
					State so = (State)ar.AsyncState;
					int bytes = 0;
					lock (_socket)
					{
						if (_socket == null) return;
						bytes = _socket.EndReceiveFrom(ar, ref epFrom);
					}
					byte[] data = null;
					if (bytes > 0)
					{
						data = so.buffer.Take(bytes).ToArray();
					}
					lock (_socket)
					{
						if (_socket == null) return;
						_socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
					}
					if (data != null)
					{
						lock (message)
						{
							message.Enqueue(data);
						}
					}
				}
				catch (System.ObjectDisposedException e)
				{
					Console.WriteLine("Async thread failed on Exception." + e.Message);
					// Ignore...
				}
			}, state);
			if (canStart.IsCompleted)
			{
				return;
			}
			Console.WriteLine("Async thread started.");
		}
	

        public void Dispose()
        {
			lock (_socket)
			{
				_socket.Shutdown(SocketShutdown.Both);
				_socket.Close();
			}
		}
	}

}


