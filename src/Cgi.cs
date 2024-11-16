using List;
using MainStructures;

namespace Cgi
{
	public class InterpreterManager
	{
		static LinkedList.ListHead interpreters = new LinkedList.ListHead();

		public static void InterpreterAdd(string ext, string path)
		{
			Interpreter inInterpreter = new Interpreter(ext, path);
			LinkedList.list_add_tail(ref inInterpreter.list, ref interpreters);
		}

		public static void CgiMain(Client cl, PathInfo pi, string url)
		{
			Interpreter? ip = pi.ip; // Assuming ip is set in PathInfo.
			ClearEnv();

			SetEnv("PATH", MainStructure.conf.cgi_path);

			foreach (EnvVar ev in GetProcessVars(cl, pi))
			{
				if (string.IsNullOrEmpty(ev.Value))
					continue;

				SetEnv(ev.Name, ev.Value);
			}

			if (!ChangeDir(pi.Root))
			{
				if (ip != null)
				{
					Execute(ip.Path, pi.Phys);
				}
				else
				{
					Execute(pi.Phys);
				}
			}
			else
			{
				Console.WriteLine($"Status: 500 Internal Server Error\r\n\r\n" +
								  $"Unable to launch the requested CGI program:\n" +
								  $"  {(ip != null ? ip.Path : pi.Phys)}: Error: Unable to change directory.");
			}
		}

		public static void CgiHandleRequest(Client cl, string url, PathInfo pi)
		{
			const uint Mode = S_IFREG | S_IXOTH;

			if (pi.ip == null && (pi.stat.st_mode & Mode) != Mode)
			{
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

		public static bool CheckCgiPath(PathInfo pi, string url)
		{
			foreach (Interpreter ip in LinkedList.Enumerate<Interpreter>(interpreters))
			{
				int len = ip.Ext.Length;
				string path = pi.Phys;

				if (len >= path.Length)
					continue;

				if (path.Substring(path.Length - len) != ip.Ext)
					continue;

				pi.ip = ip;
				return true;
			}

			pi.ip = null;

			if (!string.IsNullOrEmpty(MainStructure.conf.cgi_docroot_prefix))
			{
				return PathMatch(MainStructure.conf.cgi_docroot_prefix, pi.Phys);
			}

			return false;
		}

		public static MainStructure.dispatchHandler CgiDispatcher => new MainStructure.dispatchHandler
		{
			script = true,
			checkPath = CheckCgiPath,
			handleRequest = CgiHandleRequest
		};
	}
}
