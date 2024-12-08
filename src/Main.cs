using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Listen;
using List;
using MainStructures;
using FileHandling;
using System.Net;
using System.Text;

class Program
{
    private static Listener listener = new Listener();
    static bool nofork = false;
    static int bound = 0;

    static void Main(string[] args)
    {
        FileHandler file = new FileHandler();
        init_defaults_pre();
        file.dispatchAdd(ref MainStructure.cgiDispatch);

        ParseArgs(args);
        if (MainStructure.conf.docroot == null)
        {
            try
            {
                MainStructure.conf.docroot = Path.GetFullPath("./www");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: Unable to determine work dir: {ex.Message}");
                Environment.Exit(1);
            }
        }

        if (bound == 0)
        {
            Console.Error.WriteLine("Error: No sockets bound, unable to continue");
            Environment.Exit(1);
        }

        if (!nofork)
        {
            ForkProcess();
        }
        RunServer();
    }

    static void init_defaults_pre()
    {
        MainStructure.conf.scriptTimeout = 60;
        MainStructure.conf.networkTimeout = 30;
        MainStructure.conf.httpKeepalive = 20;
        MainStructure.conf.maxScriptRequests = 3;
        MainStructure.conf.maxConnections = 100;
        MainStructure.conf.cgiPrefix = "/cgi-bin";
        MainStructure.conf.cgiPath = "/sbin:/usr/sbin:/bin:/usr/bin";
        MainStructure.conf.realm = "Protected Area";
        MainStructure.conf.cgiAlias = new LinkedList.ListHead();
    }
    static void usage(string name)
    {
        Console.Write($@"
         _____ _   _ _____ _____ ____  ____  
        | ____| | | |_   _|_   _|  _ \|  _ \ 
        |  _| | |_| | | |   | | | |_) | | | |
        | |___|  _  | | |   | | |  __/| |_| |
        |_____|_| |_| |_|   |_| |_|   |____/ 

        ");
        Console.Write($@"
        Usage: {name} -p [addr:]port -h docroot
            -f               Do not fork to background
            -c file          Configuration file, default is '/etc/eserv.conf'
            -p [addr:]port   Bind to specified address and port, multiple allowed
            -h directory     Specify the document root, default is '.'               
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
                    if (i + 1 < args.Length)
                    {
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
                        MainStructure.conf.docroot = Path.GetFullPath(path);
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
        string currentExecutable = Assembly.GetEntryAssembly().Location;

        var startInfo = new ProcessStartInfo
        {
            FileName = currentExecutable,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            var process = Process.Start(startInfo);

            using (var nullStream = File.Open("/dev/null", FileMode.Open))
            {
                process.StandardOutput.BaseStream.CopyTo(nullStream);
                process.StandardError.BaseStream.CopyTo(nullStream);
            }

            process.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Environment.Exit(1);
        }

    }

    static async Task RunServer()
    {
        Listener.SetupListeners();
    }
}
