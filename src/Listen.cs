namespace Listen
{
	public class Listener
	{
		public static int BindSocket(string host, string port)
		{
			Console.WriteLine($"Binding socket to host: {host}, port: {port}");
			return 1;
		}
	}
}
