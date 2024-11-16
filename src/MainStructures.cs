using System.Collections.Generic;
using List;

namespace MainStructures
{
	public class MainStructure
	{
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
		

		public struct Client {
			public LinkedList.ListHead list;
			public int refcount;
			public int id;

			public int requests;
			public bool tls;
			public int http_code;
			public enum clientState state;
			public Dispatch dispatch;


		}

		public static Config conf;
	}
}
