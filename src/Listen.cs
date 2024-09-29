using List;

namespace Listen
{
	public class Listener
	{
		public struct ListenerConfig {
			public LinkedList.ListHead list;
			public int socket;
			public int n_clients;
		}

		public static int BindSocket(string host, string port) {
			Console.WriteLine($"Binding socket to host: {host}, port: {port}");
			return 1;
		}
	}
}