using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using MainStructures;
using System.Text.Json.Nodes;

namespace Proc
{
    public class ProcessManager
    {
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
            VAR_REMOTE_NAME,
            VAR_REMOTE_ADDR,
            VAR_REMOTE_PORT,
            __VAR_MAX
        }

        public class ProcHeaderEnv
        {
            public string Name { get; set; }
            public ExtraVars Index { get; set; }

            public ProcHeaderEnv(string name, ExtraVars index)
            {
                Name = name;
                Index = index;
            }
        }

        public static ProcHeaderEnv[] ProcHeaderEnvArray = new ProcHeaderEnv[]
        {
            new ProcHeaderEnv("HTTP_ACCEPT", ExtraVars._VAR_GW),
            new ProcHeaderEnv("HTTP_ACCEPT_CHARSET", ExtraVars._VAR_SOFTWARE),
            new ProcHeaderEnv("HTTP_ACCEPT_ENCODING", ExtraVars.VAR_SCRIPT_NAME),
            new ProcHeaderEnv("HTTP_ACCEPT_LANGUAGE", ExtraVars.VAR_SCRIPT_FILE),
            new ProcHeaderEnv("HTTP_AUTHORIZATION", ExtraVars.VAR_DOCROOT),
            new ProcHeaderEnv("HTTP_CONNECTION", ExtraVars.VAR_QUERY),
            new ProcHeaderEnv("HTTP_COOKIE", ExtraVars.VAR_REQUEST),
            new ProcHeaderEnv("HTTP_HOST", ExtraVars.VAR_PROTO),
            new ProcHeaderEnv("HTTP_ORIGIN", ExtraVars.VAR_METHOD),
            new ProcHeaderEnv("HTTP_REFERER", ExtraVars.VAR_PATH_INFO),
            new ProcHeaderEnv("HTTP_USER_AGENT", ExtraVars.VAR_USER),
            new ProcHeaderEnv("HTTP_X_HTTP_METHOD_OVERRIDE", ExtraVars.VAR_HTTPS),
            new ProcHeaderEnv("HTTP_AUTH_USER", ExtraVars.VAR_REDIRECT),
            new ProcHeaderEnv("HTTP_AUTH_PASS", ExtraVars.VAR_SERVER_NAME),
            new ProcHeaderEnv("CONTENT_TYPE", ExtraVars.VAR_SERVER_ADDR),
            new ProcHeaderEnv("CONTENT_LENGTH", ExtraVars.VAR_SERVER_PORT)
        };

        public class EnvVar
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public EnvVar(string name, string value = null)
            {
                Name = name;
                Value = value;
            }
        }

        public EnvVar[] ExtraVarsArray = new EnvVar[]
        {
            new EnvVar("GATEWAY_INTERFACE", "CGI/1.1"),
            new EnvVar("SERVER_SOFTWARE", "uhttpd"),
            new EnvVar("SCRIPT_NAME"),
            new EnvVar("SCRIPT_FILENAME"),
            new EnvVar("DOCUMENT_ROOT"),
            new EnvVar("QUERY_STRING"),
            new EnvVar("REQUEST_URI"),
            new EnvVar("SERVER_PROTOCOL"),
            new EnvVar("REQUEST_METHOD"),
            new EnvVar("PATH_INFO"),
            new EnvVar("REMOTE_USER"),
            new EnvVar("HTTPS"),
            new EnvVar("REDIRECT_STATUS", "some_redirect_status"),
            new EnvVar("SERVER_NAME", "localhost"),
            new EnvVar("SERVER_ADDR", "127.0.0.1"),
            new EnvVar("SERVER_PORT", "80"),
            new EnvVar("REMOTE_HOST", "192.168.1.1"),
            new EnvVar("REMOTE_ADDR", "192.168.1.1"),
            new EnvVar("REMOTE_PORT", "8080")
        };
        public MainStructure.envVar GetProcessVars(MainStructure.Client cl, MainStructure.PathInfo pi)
        {
            MainStructure.httpRequest req = cl.request;
            MainStructure.envVar vars = new MainStructure.envVar();
            JsonArray data = cl.hdr;
            string url;
            int len;
            int i;

            return vars;
        }

        public bool CreateProcess(MainStructure.Client cl, MainStructure.PathInfo pi, string url,
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
                    var process = new System.Diagnostics.Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "child_process",
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating process: {ex.Message}");
                    return false;
                }
            }

            return true;
        }
    }
}
