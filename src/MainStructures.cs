using System.Collections.Generic;
using System.Net;
using System.Text.Json.Nodes;
using List;

namespace MainStructures
{
	public class MainStructure
	{
		public struct Interpreter {
			public LinkedList.ListHead list;
			string path;
			string ext;
		}
		public struct Config
		{
			public string? docroot;
			public string? realm;
			public string? file;
			public string? error_handler;
			public string? cgi_prefix;
			public string? cgi_docroot_prefix;
			public string? cgi_path;
			public string? dirlist_charset;
			public bool no_symlinks;
			public bool no_dirlists;
			public int network_timeout;
			public bool rfc1918_filter;
			public bool tls_redirect;
			public bool tcp_keepalive;
			public int max_script_requests;
			public int max_connections;
			public int http_keepalive;
			public int script_timeout;
			public int cgi_prefix_len;
			public int events_retry;
			public LinkedList.ListHead cgi_alias;
		}
		public enum clientState {
			CLIENT_STATE_INIT,
			CLIENT_STATE_HEADER,
			CLIENT_STATE_DATA,
			CLIENT_STATE_DONE,
			CLIENT_STATE_CLOSE,
			CLIENT_STATE_CLEANUP

		};

		public enum httpMethod {
			HTTP_GET,
			HTTP_POST,
			HTTP_HEAD,
			HTTP_OPTIONS,
			HTTP_PUT,
			HTTP_PATCH,
			HTTP_DELETE
		}

		public enum httpVersion {
			HTTP_VER_0_9,
			HTTP_VER_1_0,
			HTTP_VER_1_1
		}

		public enum httpUserAgent {
			UA_UNKNOWN,
			UA_GECKO,
			UA_CHROME,
			UA_SAFARI,
			UA_MSIE,
			UA_KONQUEROR,
			UA_OPERA,
			UA_MSIE_OLD,
			UA_MSIE_NEW
		}

		public struct httpRequest {
			public httpMethod method;
			public httpVersion version;
			public httpUserAgent ua;
			int redirectStatus;
			int contentLength;
			bool expectCont;
			bool connectionClose;
			bool disableChunked;
			int transferChunked;
		}
		public struct PathInfo {
			public string root;
			public string phys;
			public string name;
			public string info;
			public string query;
			bool redirected;
			public Stat stat;
			Interpreter ip;
		}

		public struct envVar {
			string name;
			string value;
		}
		
		public class Dispatch {
			public Func<Client, string, int, int> DataSend { get; set; }
			public Action<Client> DataDone { get; set; }
			public Action<Client> WriteCb { get; set; }
			public Action<Client> CloseFds { get; set; }
			public Action<Client> Free { get; set; }

			public object ReqData { get; set; }
			public Action<Client> ReqFree { get; set; }

			public bool DataBlocked { get; set; }
			public bool NoCache { get; set; }

			public JsonArray FileHdr { get; set; }
			public int FileFd { get; set; }
			public DispatchProc Proc { get; set; }
		}

		public struct Client {
			public LinkedList.ListHead list;
			public int refcount;
			public int id;

			public int requests;
			public httpRequest request;
			public bool tls;
			public int http_code;
			public clientState state;
			public Dispatch dispatch;
		}

		public struct dispatchHandler {
			public LinkedList.ListHead list;
			public bool script;
		public Func<string, bool>? checkUrl;
		public Func<PathInfo, string, bool>? checkPath;
		public Action<Client, string, PathInfo>? handleRequest;
		}

		public static Config conf;
	}
}
