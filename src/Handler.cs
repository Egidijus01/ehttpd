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
        public static async Task HandleIncomingConnections(HttpListener listen)
        {
            try
            {
                listen.Start();
                bool runServer = true;

                while (runServer)
                {
                    try
                    {
                        HttpListenerContext ctx = await listen.GetContextAsync();
                        HttpListenerRequest req = ctx.Request;
                        HttpListenerResponse resp = ctx.Response;

                        Console.WriteLine($"Request URL: {req.Url}");
                        Console.WriteLine($"Request Method: {req.HttpMethod}");
                        Console.WriteLine($"User Host Name: {req.UserHostName}");
                        Console.WriteLine($"User Agent: {req.UserAgent}");
                        Console.WriteLine();

                        string requestPath = req.Url.AbsolutePath;

                        if (requestPath.StartsWith("/api"))
                        {
                            await API.APIDispatcher.Dispatch(ctx);
                        }
                        else
                        {
                            if (requestPath == "/")
                            {
                                requestPath = "/index.html";
                            }
                            string sanitizedPath = requestPath.TrimStart('/');
                            string fullPath = Path.Combine(MainStructure.conf.docroot, sanitizedPath);

                            if (File.Exists(fullPath))
                            {
                                byte[] fileData = await File.ReadAllBytesAsync(fullPath);
                                resp.ContentType = "text/html";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = fileData.LongLength;

                                await resp.OutputStream.WriteAsync(fileData, 0, fileData.Length);
                            }
                            else
                            {
                                string errorMessage = "<html><body><h1>404 Not Found</h1></body></html>";
                                byte[] data = Encoding.UTF8.GetBytes(errorMessage);
                                resp.StatusCode = (int)HttpStatusCode.NotFound;
                                resp.ContentType = "text/html";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = data.LongLength;

                                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                            }
                        }
                        resp.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error handling request: {ex.Message}");
                    }
                }

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
