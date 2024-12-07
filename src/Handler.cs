using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using List;
using MainStructures;

namespace Handler
{
    public class Handle
    {
        public static string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";

        public static async Task HandleIncomingConnections(HttpListener listen)
        {
            Console.WriteLine("Starting server...");

            // Add prefix to HttpListener, this binds it to a specific address and port
            listen.Prefixes.Add("http://127.0.0.1:8523/");
            
            try
            {
                // Start the listener before entering the loop
                listen.Start();
                Console.WriteLine("Server started. Listening on http://127.0.0.1:8523/");

                bool runServer = true;

                // Keep the server running while it should
                while (runServer)
                {
                    try
                    {
                        HttpListenerContext ctx = await listen.GetContextAsync();
                        HttpListenerRequest req = ctx.Request;
                        HttpListenerResponse resp = ctx.Response;

                        // Log request details
                        Console.WriteLine($"Request URL: {req.Url}");
                        Console.WriteLine($"Request Method: {req.HttpMethod}");
                        Console.WriteLine($"User Host Name: {req.UserHostName}");
                        Console.WriteLine($"User Agent: {req.UserAgent}");
                        Console.WriteLine();

                        // If shutdown requested, stop the server
                        if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                        {
                            Console.WriteLine("Shutdown requested");
                            runServer = false;
                        }

                        // Increment page views, if not shutdown
                        if (req.Url.AbsolutePath != "/favicon.ico")
                        {
                            MainStructure.conf.pageViews += 1;
                        }

                        // Determine if submit button should be disabled
                        string disableSubmit = !runServer ? "disabled" : "";

                        // Prepare the response data
                        byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, MainStructure.conf.pageViews, disableSubmit));

                        // Set response headers
                        resp.ContentType = "text/html";
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;

                        // Write the response to the output stream asynchronously
                        await resp.OutputStream.WriteAsync(data, 0, data.Length);
                        resp.Close();
                    }
                    catch (Exception ex)
                    {
                        // Catch and log any exceptions that occur during request processing
                        Console.WriteLine($"Error handling request: {ex.Message}");
                    }
                }

                // After loop ends, stop the listener gracefully
                Console.WriteLine("Server is stopping...");
                listen.Stop();
                listen.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting listener: {ex.Message}");
            }
        }
    }
}
