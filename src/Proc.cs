using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using MainStructures;
using System.Text.Json.Nodes;

namespace Proc
{
	public class Process
	{
		private static readonly int HDR_accept = 1;
		private static readonly int HDR_accept_charset = 2;
		private static readonly int HDR_accept_encoding = 3;
		private static readonly int HDR_accept_language = 4;
		private static readonly int HDR_authorization = 5;
		private static readonly int HDR_connection = 6;
		private static readonly int HDR_cookie = 7;
		private static readonly int HDR_host = 8;
		private static readonly int HDR_origin = 9;
		private static readonly int HDR_referer = 10;
		private static readonly int HDR_user_agent = 11;
		private static readonly int HDR_x_http_method_override = 12;
		private static readonly int HDR_http_auth_user = 13;
		private static readonly int HDR_http_auth_pass = 14;
		private static readonly int HDR_content_type = 15;
		private static readonly int HDR_content_length = 16;

		// Array of Tuples representing the structure
		private static readonly (string name, int idx)[] procHeaderEnv = new (string, int)[]
		{
			{ "HTTP_ACCEPT", HDR_accept },
			{ "HTTP_ACCEPT_CHARSET", HDR_accept_charset },
			{ "HTTP_ACCEPT_ENCODING", HDR_accept_encoding },
			{ "HTTP_ACCEPT_LANGUAGE", HDR_accept_language },
			{ "HTTP_AUTHORIZATION", HDR_authorization },
			{ "HTTP_CONNECTION", HDR_connection },
			{ "HTTP_COOKIE", HDR_cookie },
			{ "HTTP_HOST", HDR_host },
			{ "HTTP_ORIGIN", HDR_origin },
			{ "HTTP_REFERER", HDR_referer },
			{ "HTTP_USER_AGENT", HDR_user_agent },
			{ "HTTP_X_HTTP_METHOD_OVERRIDE", HDR_x_http_method_override },
			{ "HTTP_AUTH_USER", HDR_http_auth_user },
			{ "HTTP_AUTH_PASS", HDR_http_auth_pass },
			{ "CONTENT_TYPE", HDR_content_type },
			{ "CONTENT_LENGTH", HDR_content_length }
		};

		public enum ExtraVars
		{
			_VAR_GW,
			_VAR_SOFTWARE,
			VAR_SCRIPT_NAME,
			VAR_SCRIPT_FILE,
			VAR_DOCROOT,
			VAR_QUERY,
			VAR_REQUEST,
			VAR_PROTO,
			VAR_METHOD,
			VAR_PATH_INFO,
			VAR_USER,
			VAR_HTTPS,
			VAR_REDIRECT,
			VAR_SERVER_NAME,
			VAR_SERVER_ADDR,
			VAR_SERVER_PORT,
			VAR_REMOTE_ADDR,

			__VAR_MAX
		}

		public MainStructure.envVar GetProccessVars(MainStructure.Client Cl, MainStructure.PathInfo pi) 
		{
			MainStructure.httpRequest req = Cl.request;
			MainStructure.envVar vars = new MainStructure.envVar();
			JsonArray data = Cl.hdr.head;
			string url;
			int len;
			int i;

			return vars;
		}

		public bool create_process(MainStructure.Client cl, MainStructure.PathInfo pi, string url,
								Action<MainStructure.Client, MainStructure.PathInfo, string> callback)
		{
			var dispatch = cl.dispatch;
			var proc = dispatch.Proc;
			proc.StatusCode = 200;
			proc.StatusMessage = "OK";

			using (var pipeIn = new AnonymousPipeServerStream(PipeDirection.In))
			using (var pipeOut = new AnonymousPipeServerStream(PipeDirection.Out))
			{
				try
				{
					var process = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							FileName = "child_process",
							RedirectStandardInput = true,
							RedirectStandardOutput = true,
							UseShellExecute = false,  // UseShellExecute must be false to redirect
							CreateNoWindow = true
						}
					};
					process.Start();
				}
				catch (Exception ex)
				{
					// Handle any exceptions that occur during process creation
					Console.WriteLine($"Error creating process: {ex.Message}");
					return false;
				}
			}

			return true;
		}
	}
}
