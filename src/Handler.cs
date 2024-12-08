using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

                        if (!Regex.IsMatch(requestPath, @"\.[a-zA-Z0-9]+$"))
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
                                string fileExtension = Path.GetExtension(fullPath).ToLower();
                                string contentType = GetContentType(fileExtension);

                                byte[] fileData = await File.ReadAllBytesAsync(fullPath);
                                resp.ContentType = contentType;
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = fileData.LongLength;

                                await resp.OutputStream.WriteAsync(fileData, 0, fileData.Length);
                            }
                            else
                            {
                                string errorMessage = "{\"error\": \"File not found\"}";
                                byte[] errorData = Encoding.UTF8.GetBytes(errorMessage);

                                resp.StatusCode = (int)HttpStatusCode.NotFound;
                                resp.ContentType = "application/json";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = errorData.Length;

                                await resp.OutputStream.WriteAsync(errorData, 0, errorData.Length);
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

        private static string GetContentType(string extension)
        {
            switch (extension)
            {
                case ".html": return "text/html";
                case ".css": return "text/css";
                case ".js": return "application/javascript";
                case ".json": return "application/json";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                case ".svg": return "image/svg+xml";
                case ".woff": 
                case ".woff2": return "font/woff2";
                case ".ttf": return "font/ttf";
                case ".otf": return "font/otf";
                default: return "application/octet-stream";
            }
        }
    }
}
