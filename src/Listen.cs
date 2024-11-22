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
		private static LinkedList<ListenerConfig> listeners;

		public Listener()
		{
			listeners = new LinkedList<ListenerConfig>();
		}

		public struct AddressInfo
		{
			public AddressFamily Family;
			public SocketType SocketType;
			public SocketFlags Flags;
		}

		private class ListenerConfig
		{
			public Socket Socket { get; set; }
			public IPEndPoint Addr { get; set; }
			public int NClients { get; set; }
			public bool Blocked { get; set; }
			public bool Tls { get; set; }
		}

		public static int BindSocket(string host, string port, bool tls)
		{
			int bound = 0;

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
					var socket = new Socket(endpoint.AddressFamily, hints.SocketType, ProtocolType.Tcp);

					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

					if (endpoint.AddressFamily == AddressFamily.InterNetworkV6)
					{
						socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, true);
					}

					socket.Bind(endpoint);
					socket.Listen(10);

					var listener = new ListenerConfig
					{
						Socket = socket,
						Addr = endpoint,
						Tls = tls,
						NClients = 0,
						Blocked = false
					};

					listeners.AddLast(listener);
					bound++;
				}

				return bound;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error binding socket: {ex.Message}");
				return 0;
			}
		}

		public static void SetupListeners()
		{
			foreach (var listener in listeners)
			{
				try
				{
					if (MainStructure.conf.tcp_keepalive > 0)
					{
						listener.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

						// Simulated keep-alive settings - actual implementation may differ based on OS
						// Usually involves platform-specific IOCTL calls.
						Console.WriteLine("KeepAlive options set (pseudo-code, adjust for OS specifics).");
					}
				}
				catch (SocketException ex)
				{
					Console.WriteLine($"Error setting TCP keepalive options: {ex.Message}");
				}
			}
		}

		public void RunListeners()
		{
			foreach (var listener in listeners)
			{
				listener.Socket.BeginAccept(AcceptCallback, listener.Socket);
			}
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			try
			{
				var listenerSocket = (Socket)ar.AsyncState;
				var clientSocket = listenerSocket.EndAccept(ar);

				Console.WriteLine($"Accepted new connection: {clientSocket.RemoteEndPoint}");

				// Begin accepting the next connection
				listenerSocket.BeginAccept(AcceptCallback, listenerSocket);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in AcceptCallback: {ex.Message}");
			}
		}
	}
}
