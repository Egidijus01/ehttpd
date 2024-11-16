using List;
using MainStructures;

namespace Cgi {

	public class InterpreterManager {
		static LinkedList.ListHead interpreters = new LinkedList.ListHead();

		public static void InterpreterAdd(string ext, string path) {
			Interpreter inInterpreter = new Interpreter(ext, path);
			LinkedList.list_add_tail(ref inInterpreter.list, ref interpreters);
		}
		public static void cgi_main(Client Cl, PathInfo Pi, string url) {
			Interpreter Ip = new Interpreter();
			EnvVar var = new EnvVar();
			ClearEnv();

			SetEnv("PATH", MainStructure.conf.cgi_path);

			foreach (var in GetProccessVars(cl, Pi)) {
				if (string.IsNullOrEmpty(var.Value)) {
					continue;
				}

				SetEnv(var.Name, var.Value);
			}

			if (!ChangeDir(Pi.Root)) {
				if (Ip) {
					Execute(Ip.Path, Pi.Phys);
				} else {
					Execute(Pi.Phys);
				}
			}
			Console.WriteLine("Status: 500 Internal Server Error\r\n\r\n" +
				"Unable to launch the requested CGI program:\n" +
				"  {0}: {1}\n", Ip != null ? Ip.Path : pi.Phys, "Error: Unable to change directory.");
		}

		public static void cgi_handle_request(Client Cl, string url, PathInfo Pi) {
			uint mode = S_IFREG | S_IXOTH;
			string escaped_url;

			if (!Pi.ip && !((Pi.stat.st_mode & mode) == mode)) {
				escaped_url = htmlescape(url);
				ClientError(Cl, 403, "Forbidden",
							"You don't have permission to access {0} on this server.",
							escaped_url ? escaped_url : "the url");
				return;
			}

			if (!create_proccess(Cl, Pi, url, cgi_main)) {
				ClientError(Cl, 500, "Internal Server Error",
							"Failed to create CGI process");
				return;
			}

			return;
		}

		public static bool check_cgi_path(PathInfo Pi, string url) {
			Interpreter Ip = new Interpreter;
			string path = Pi.phys;
			int path_len = path.Length;

			list_for_each_entry(Ip, interpreters, list) {
				int len = Ip.ext.Length;

				if (len >= path_len) {
					continue;
				}
				if (!(path+path_len - len == Ip.ext)) {
					continue;
				}
				Pi.ip = ip;
				return true;
			}
			Pi.ip = null;
			
			if (MainStructure.conf.cgi_docroot_path) {
				return PathMatch(MainStructure.conf.cgi_docroot_path, Pi.phys);
			}
			return false;
		} 
	}
}
