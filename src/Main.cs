using System;
using System.IO;
using System.Diagnostics;
using Listen;
using List;
using MainStructures;
using File;

class Program
{
	static bool nofork = false;
	static string docroot = null;
	static int bound = 0;

	static void Main(string[] args)
	{
		File.File file = new File.File();

		init_defaults_pre();
		file.dispatchAdd(ref MainStructure.cgiDispatch);

		ParseArgs(args);

		if (docroot == null)
		{
			try {
				docroot = Path.GetFullPath(".");
			} catch (Exception ex) {
				Console.Error.WriteLine($"Error: Unable to determine work dir: {ex.Message}");
				Environment.Exit(1);
			}
		}

		if (bound == 0) {
			Console.Error.WriteLine("Error: No sockets bound, unable to continue");
			Environment.Exit(1);
		}

		if (!nofork) {
			ForkProcess();
		}

		RunServer();
	}

	static void init_defaults_pre() {
		MainStructure.conf.script_timeout = 60;
		MainStructure.conf.network_timeout = 30;
		MainStructure.conf.http_keepalive = 20;
		MainStructure.conf.max_script_requests = 3;
		MainStructure.conf.max_connections = 100;
		MainStructure.conf.cgi_prefix = "/cgi-bin";
		MainStructure.conf.cgi_path = "/sbin:/usr/sbin:/bin:/usr/bin";
		MainStructure.conf.realm = "Protected Area";
		MainStructure.conf.cgi_alias = new LinkedList.ListHead();
	}
	static void usage(string name) {
		Console.Write($@"
		 _____ _   _ _____ _____ ____  ____  
		| ____| | | |_   _|_   _|  _ \|  _ \ 
		|  _| | |_| | | |   | | | |_) | | | |
		| |___|  _  | | |   | | |  __/| |_| |
		|_____|_| |_| |_|   |_| |_|   |____/ 

		");
		Console.Write($@"
		Usage: {name} -p [addr:]port -h docroot
			-f				 Do not fork to background
			-c file			 Configuration file, default is '/etc/eserv.conf'
			-p [addr:]port	 Bind to specified address and port, multiple allowed
			-h directory	 Specify the document root, default is '.'				 
		");
	}

	static void ParseArgs(string[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			string optarg = args[i];

			switch (optarg)
			{
				case "-p":
					if (i + 1 < args.Length) {
						string port = args[++i];
						bound += AddListenerArg(port);
					}
					break;

				case "-h":
					if (i + 1 < args.Length)
					{
						string path = args[++i];
						if (!Directory.Exists(path))
						{
							Console.Error.WriteLine($"Error: Invalid directory {path}");
							Environment.Exit(1);
						}
						docroot = Path.GetFullPath(path);
					}
					break;

				case "-f":
					nofork = true;
					break;

				case "-d":
					if (i + 1 < args.Length)
					{
						string decoded = DecodeUrl(args[++i]);
						Console.WriteLine(decoded);
					}
					break;

				default:
					usage(args[0]);
					break;
			}
		}
	}

	static int AddListenerArg(string arg)
	{
		string host = null;
		string port = arg;
		int l;

		int index = arg.LastIndexOf(":");

		if (index != -1)
		{
			host = arg.Substring(0, index);
			port = arg.Substring(index + 1);
		}

		if (host != null && host.StartsWith("[") && host.EndsWith("]"))
		{
			l = host.Length;
			host = host.Substring(1, l - 2);
		}
		return Listener.BindSocket(host, port, false);
	}

	static string DecodeUrl(string url)
	{
		// Placeholder URL decode logic
		return Uri.UnescapeDataString(url);
	}

	static void ForkProcess()
	{
		try
		{
			ProcessStartInfo startInfo = new ProcessStartInfo()
			{
				FileName = "/bin/sh",
				Arguments = "-c \"sleep 60\"",
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};
			Process process = Process.Start(startInfo);

			// Redirect output (equivalent to `dup2` to /dev/null)
			process.StandardOutput.Dispose();
			process.StandardError.Dispose();
			process.StandardInput.Dispose();

			Console.WriteLine("Forked process");
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"Error: {ex.Message}");
			Environment.Exit(1);
		}
	}

	static void RunServer()
	{
		// Placeholder for running the server logic
		Console.WriteLine("Running server...");
	}
}
