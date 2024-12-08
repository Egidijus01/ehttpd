using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using List;
using MainStructures;
using Handler;

namespace Listen
{
    public class Listener
    {
        private static LinkedList<ListenerConfig> listeners;

        public Listener()
        {
            listeners = new LinkedList<ListenerConfig>();
        }

        public struct AddressInfo
        {
            public AddressFamily Family;
            public SocketType SocketType;
            public SocketFlags Flags;
        }

        private class ListenerConfig
        {
            public HttpListener listener { get; set; }
            public bool Tls { get; set; }
            public string url { get; set; }
        }

        public static int BindSocket(string host, string port, bool tls)
        {
            int bound = 0;
            string prefix = tls ? "https://" : "http://";
            var hints = new AddressInfo
            {
                Family = AddressFamily.Unspecified,
                SocketType = SocketType.Stream,
                Flags = SocketFlags.None
            };

            try
            {
                var addresses = Dns.GetHostAddresses(host);
                foreach (var address in addresses)
                {
                    HttpListener listen = new HttpListener();
                    string url = prefix + host + ":" + port + "/";

                    listen.Prefixes.Add(url);

                    var listener = new ListenerConfig
                    {
                        listener = listen,
                        url = url,
                        Tls = tls

                    };

                    listeners.AddLast(listener);
                    bound++;

                }
                return bound;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error binding socket: {ex.Message}");
                return 0;
            }
        }

        public static async void SetupListeners()
        {
            foreach (var listener in listeners)
            {
                try
                {
                    Task listenTask = Handle.HandleIncomingConnections(listener.listener);
                    listenTask.GetAwaiter().GetResult();

                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Error setting TCP keepalive options: {ex.Message}");
                }
            }
        }
    }
}
