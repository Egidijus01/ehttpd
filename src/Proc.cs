namespace Proc
{
	public class Process
	{
		// Array of proc_header_env, assuming HDR_* are defined somewhere
		private static readonly (string HeaderName, string HeaderValue)[] procHeaderEnv = 
		{
			("HTTP_ACCEPT", HDR_accept),
			("HTTP_ACCEPT_CHARSET", HDR_accept_charset),
			("HTTP_ACCEPT_ENCODING", HDR_accept_encoding),
			("HTTP_ACCEPT_LANGUAGE", HDR_accept_language),
			("HTTP_AUTHORIZATION", HDR_authorization),
			("HTTP_CONNECTION", HDR_connection),
			("HTTP_COOKIE", HDR_cookie),
			("HTTP_HOST", HDR_host),
			("HTTP_ORIGIN", HDR_origin),
			("HTTP_REFERER", HDR_referer),
			("HTTP_USER_AGENT", HDR_user_agent),
			("HTTP_X_HTTP_METHOD_OVERRIDE", HDR_x_http_method_override),
			("HTTP_AUTH_USER", HDR_http_auth_user),
			("HTTP_AUTH_PASS", HDR_http_auth_pass),
			("CONTENT_TYPE", HDR_content_type),
			("CONTENT_LENGTH", HDR_content_length)
		};

		public enum ExtraVars {
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

		public EnvVar GetProccessVars(Client Cl, PathInfo pi) {
			HttpRequest req = Cl.request;
			
		}
	}
}
