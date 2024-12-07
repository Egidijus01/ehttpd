using MainStructures;
using System;
using System.Collections.Generic;

namespace Cgi
{
	public class InterpreterManager
	{
		static List<MainStructure.Interpreter> interpreters = new List<MainStructure.Interpreter>();

		public static void InterpreterAdd(string ext, string path)
		{
			MainStructure.Interpreter inInterpreter = new MainStructure.Interpreter(ext, path);
			interpreters.Add(inInterpreter);
		}

		public static void CgiMain(MainStructure.Client cl, MainStructure.PathInfo pi, string url)
		{
			MainStructure.Interpreter? ip = pi.ip;
			ClearEnv();

			SetEnv("PATH", MainStructure.conf.cgi_path);

			foreach (MainStructure.envVar ev in GetProcessVars(cl, pi))
			{
				if (string.IsNullOrEmpty(ev.value))
					continue;

				SetEnv(ev.name, ev.value);
			}

			if (!ChangeDir(pi.root))
			{
				if (ip != null) {
					Execute(ip.Value.Path);
				} else {
					Execute(pi.phys);
				}
			}
			Console.WriteLine($"Status: 500 Internal Server Error\r\n\r\n" +
								$"Unable to launch the requested CGI program:\n" +
								$"  {(ip != null ? ip.Value.Path : pi.phys)}: Error: Unable to change directory.");
		}

		public static void CgiHandleRequest(MainStructure.Client cl, string url, MainStructure.PathInfo pi)
		{
		    const FileAttributes Mode = FileAttributes.Normal | FileAttributes.System; // Change this to appropriate attributes

		    if (pi.ip.Equals(default(MainStructure.Interpreter)) && !pi.stat.Attributes.HasFlag(Mode)) {
				string? escapedUrl = HtmlEscape(url);
				ClientError(cl, 403, "Forbidden",
							$"You don't have permission to access {escapedUrl ?? "the URL"} on this server.");
				return;
			}

			if (!CreateProcess(cl, pi, url, CgiMain))
			{
				ClientError(cl, 500, "Internal Server Error", "Failed to create CGI process");
			}
		}

		public static bool CheckCgiPath(MainStructure.PathInfo pi, string url)
		{
			foreach (MainStructure.Interpreter ip in interpreters)
			{
				int len = ip.Ext.Length;
				string path = pi.phys;

				if (len >= path.Length)
					continue;

				if (path.Substring(path.Length - len) != ip.Ext)
					continue;

				pi.ip = ip;
				return true;
			}

			pi.ip = default(MainStructure.Interpreter);

			if (!string.IsNullOrEmpty(MainStructure.conf.cgi_docroot_path))
			{
				return PathMatch(MainStructure.conf.cgi_docroot_path, pi.phys);
			}

			return false;
		}

		public static MainStructure.dispatchHandler CgiDispatcher => new MainStructure.dispatchHandler
		{
			script = true,
			checkPath = CheckCgiPath,
			handleRequest = CgiHandleRequest
		};

		private static void SetEnv(string name, string value)
		{
			Environment.SetEnvironmentVariable(name, value);
		}

		private static void ClearEnv()
		{
		}

		private static IEnumerable<MainStructure.envVar> GetProcessVars(MainStructure.Client cl, MainStructure.PathInfo pi)
		{
			return new List<MainStructure.envVar>();
		}

		private static bool ChangeDir(string path)
		{
			try
			{
				System.IO.Directory.SetCurrentDirectory(path);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static void Execute(string path, string phys)
		{
			System.Diagnostics.Process.Start(path, phys);
		}

		private static void Execute(string phys)
		{
			System.Diagnostics.Process.Start(phys);
		}

		private static string HtmlEscape(string url)
		{
			return Uri.EscapeDataString(url);
		}

		private static void ClientError(MainStructure.Client cl, int statusCode, string statusText, string message)
		{
			Console.WriteLine($"Error {statusCode}: {statusText} - {message}");
		}

		private static bool PathMatch(string prefix, string path)
		{
			return path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
		}

		private static bool CreateProcess(MainStructure.Client cl, MainStructure.PathInfo pi, string url, Action<MainStructure.Client, MainStructure.PathInfo, string> mainFunction)
		{
			mainFunction(cl, pi, url);
			return true;
		}
	}
}
