using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using List;
using MainStructures;

namespace Listen
{
	public class Listener
	{
		public struct AddressInfo
		{
			public AddressFamily Family;
			public SocketType SocketType;
			public SocketFlags Flags;
		}

		private struct ListenerConfig
		{
			public LinkedList.ListHead List;
			public FileStream Sfd;
			public IPEndPoint Addr;
			public Socket Socket;
			public int NClients;
			public bool Blocked;
			public bool Tls;
		}

		public static int BindSocket(string host, string port, bool tls)
		{
			int bound = 0;
			Socket socket = null;
			var hints = new AddressInfo
			{
				Family = AddressFamily.Unspecified,
				SocketType = SocketType.Stream,
				Flags = SocketFlags.None
			};

			try
			{
				var addresses = Dns.GetHostAddresses(host);
				foreach (var address in addresses)
				{
					var endpoint = new IPEndPoint(address, int.Parse(port));

					socket = new Socket(endpoint.AddressFamily, hints.SocketType, ProtocolType.Tcp);

					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

					if (endpoint.AddressFamily == AddressFamily.InterNetworkV6)
					{
						socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, true);
					}

					socket.Bind(endpoint);

					socket.Listen(10);

					ListenerConfig listener = new ListenerConfig
					{
						Socket = socket,
						Addr = endpoint,
						Tls = tls,
						NClients = 0,
						Blocked = false
					};

					bound++;
				}

				return bound;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error binding socket: {ex.Message}");
				return 0;
			}
			finally {
				socket?.Close();
			}
		}
	}
}